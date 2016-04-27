using System;
using System.Collections.Generic;
using System.Net.Http.Formatting;
using Newtonsoft.Json;

namespace TelegramBot.Task
{
    public interface IBotTaskArg
    {
        Guid Id { get; }
        string TaskHandlerName { get; }
        string ChatId { get; }
        TimeSpan Period { get; set; }
        IDictionary<string, string> Properties { get; set; }
    }

    [Serializable]
    public class BotTaskArg : IBotTaskArg
    {
        public Guid Id { get; set; }
        public string TaskHandlerName { get; set; }
        public string ChatId { get; set; }
        public TimeSpan Period { get; set; }
        public IDictionary<string, string> Properties { get; set; }
    }
}