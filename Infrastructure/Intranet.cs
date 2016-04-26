using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SimpleImpersonation;

namespace Infrastructure
{
    public static class Intranet
    {
        static Random _random = new Random(100000);

        public static UserInfoDto[] GetUsers()
        {
            var settings = SettingsProvider.Get();
            var cc = new CredentialCache
            {
                {new Uri("http://intranet/"), "NTLM", new NetworkCredential(settings.IntranetUserName, settings.IntranetPassword, settings.IntranetDomain)}
            };

            using (var client = new WebClient())
            {
                client.Credentials = cc;

                var data = client.DownloadData(
                    "http://intranet/components/com_comprofiler/json/get-all-users-info.php");

                var usersJson = DecodeEncodedNonAsciiCharacters(Encoding.UTF8.GetString(data));
                var users = JsonConvert.DeserializeObject<UserInfoDto[]>(usersJson);

                return users.Where(u => !string.IsNullOrWhiteSpace(u.email)).ToArray();
            }
        }

        public static UserInOfficeDto GetUserInOfficeInfo(string mail)
        {
            var settings = SettingsProvider.Get();
            var cc = new CredentialCache
            {
                {new Uri("http://intranet/"), "NTLM", new NetworkCredential(settings.IntranetUserName, settings.IntranetPassword, settings.IntranetDomain)}
            };

            using (var client = new WebClient())
            {
                client.Credentials = cc;

                var data = client.DownloadData(
                    "http://intranet/components/com_worktime/check_user_in_office.php?email=" + mail);

                var jsonUserInOfficeInfo = DecodeEncodedNonAsciiCharacters(Encoding.UTF8.GetString(data));
                var userInOfficeInfo = JsonConvert.DeserializeObject<UserInOfficeDto>(jsonUserInOfficeInfo);
                return userInOfficeInfo;
            }
        }

        public static UserInOfficeDto GetUserInOfficeInfoFromGrabber(string mailArg)
        {
            var number = _random.Next(1000);
            var inFile = @"\\vm-projects\share\in" + number + ".txt";
            var outFile = @"\\vm-projects\share\out" + number + ".txt";

            using (Impersonation.LogonUser(
                           "VM-Projects",
                           "adminga",
                           "GApioner25",
                           LogonType.NewCredentials))
            {
                var start = DateTime.Now;
                while (true)
                {
                    if (!File.Exists(inFile))
                        File.WriteAllText(inFile, mailArg);

                    if (File.Exists(outFile))
                    {
                        var data = File.ReadAllText(outFile);
                        var res = JsonConvert.DeserializeObject<UserInOfficeDto>(data);
                        File.Delete(inFile);
                        File.Delete(outFile);
                        return res;
                    }

                    if ((DateTime.Now - start).TotalSeconds > 30)
                        throw new ArgumentException("timeout");
                }
            }
        }

        static string DecodeEncodedNonAsciiCharacters(string value)
        {
            return Regex.Replace(
                value,
                @"\\u(?<Value>[a-zA-Z0-9]{4})",
                m => {
                    return ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString();
                });
        }

        static string FirstCharToUpper(string input)
        {
            if (String.IsNullOrEmpty(input))
                throw new ArgumentException("ARGH!");
            return input.First().ToString().ToUpper() + input.Substring(1);
        }
    }
}
