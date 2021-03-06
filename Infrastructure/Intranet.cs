﻿using System;
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

                return users.ToArray();
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

        private static string DecodeEncodedNonAsciiCharacters(string value)
        {
            return Regex.Replace(
                value,
                @"\\u(?<Value>[a-zA-Z0-9]{4})",
                m => {
                    return ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString();
                });
        }
    }
}
