using System;
using System.Collections.Generic;
using System.Net.Http.Formatting;
using Newtonsoft.Json;

namespace TelegramBot.Task
{
    public interface IBotTaskArg
    {
        string TaskHandlerName { get; }
        string ChatId { get; }
        TimeSpan Period { get; }
        IDictionary<string, string> Properties { get; }
    }

    [Serializable]
    public class BotTaskArg : IBotTaskArg
    {
        public string TaskHandlerName { get; set; }
        public string ChatId { get; set; }
        public TimeSpan Period { get; set; }
        public IDictionary<string, string> Properties { get; set; }
    }
}