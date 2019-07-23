using System.IO;
using Newtonsoft.Json;

namespace Infrastructure
{
    public class Settings
    {
        public string IntranetUserName { get; set; }
        public string IntranetPassword { get; set; }
        public string IntranetDomain { get; set; }

        public string BotApiKey { get; set; }
        public string Proxy { get; set; }

        public string[] AuthChatIds { get; set; }
        public string Secret { get; set; }

    }

    public static class SettingsProvider
    {
        public static Settings Get()
        {
            if (File.Exists("settings.json"))
                return JsonConvert.DeserializeObject<Settings>(File.ReadAllText("settings.json"));

            return new Settings();
        }

        public static void Set(Settings settings)
        {
            File.WriteAllText("settings.json", JsonConvert.SerializeObject(settings));
        }
    }
}