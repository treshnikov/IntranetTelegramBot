using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Threading;
using Logger;
using Newtonsoft.Json;
using Telegram.Bot;

namespace TelegramBot.Task
{
    public class BotTaskProcessor : IDisposable
    {
        private static Api _bot;
        private readonly ILogger _logger;
        private static List<IBotTaskHandler>  _handlers;
        public static List<IBotTaskArg> TaskArgs { get; set; }
        private static object TaskArgsLock = new object();

        static BotTaskProcessor()
        {
            TaskArgs = new List<IBotTaskArg>();
        }

        public BotTaskProcessor(Api bot, ILogger logger)
        {
            _bot = bot;
            _logger = logger;
            PrepareHandlers();
            LoadArgs();
            StartThreads();
        }

        private void StartThreads()
        {
            foreach (var arg in TaskArgs)
            {
                StartThreadForTaskArg(arg);
            }
        }

        private static void StartThreadForTaskArg(IBotTaskArg arg)
        {
            IBotTaskHandler handler;
            if (TryGetHandlerByName(arg.TaskHandlerName, out handler))
            {
                ThreadPool.QueueUserWorkItem(args =>
                {
                    while (true)
                    {
                        try
                        {
                            handler.Handle(_bot, arg);
                            Thread.Sleep(arg.Period);
                        }
                        catch (Exception ex)
                        {
                            Thread.Sleep(arg.Period);
                            //todo
                        }
                    }
                });
            }
        }

        public static bool TryGetHandlerByName(string name, out IBotTaskHandler handler)
        {
            foreach (var h in _handlers)
            {
                if (string.Equals(h.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    handler = h;
                    return true;
                }
            }

            handler = null;
            return false;
        }

        private void PrepareHandlers()
        {
            _handlers = new List<IBotTaskHandler>();

            var handlerType = typeof (IBotTaskHandler);
            var handlerTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => !p.IsInterface && handlerType.IsAssignableFrom(p)).ToList();


            foreach (var h in handlerTypes)
            {
                var handler = Activator.CreateInstance(h);
                _handlers.Add((IBotTaskHandler) handler);
            }
        }

        private static void LoadArgs()
        {
            lock (TaskArgsLock)
            {
                if (File.Exists("tasks.json"))
                {
                    TaskArgs =
                        new List<IBotTaskArg>(JsonConvert.DeserializeObject<BotTaskArg[]>(File.ReadAllText("tasks.json")));
                }
            }

            //TaskArgs = new List<IBotTaskArg>
            //{
            //    new BotTaskArg()
            //    {
            //        ChatId = "175592600",
            //        Period = TimeSpan.FromSeconds(5),
            //        TaskHandlerName = "время",
            //        Properties = new Dictionary<string, string>()
            //    }
            //};
        }

        public static void SaveArgs()
        {
            lock (TaskArgsLock)
            {
                File.WriteAllText("tasks.json", JsonConvert.SerializeObject(TaskArgs));
            }
        }

        public void Dispose()
        {
            SaveArgs();
        }

        public static void AddTaskArg(IBotTaskArg arg)
        {
            lock (TaskArgsLock)
            {
                TaskArgs.Add(arg);
                StartThreadForTaskArg(arg);
                SaveArgs();
            }
        }

        public static void RemoveTaskArg(string chatId, string command)
        {
            // todo - подумать как остановить поток
            lock (TaskArgsLock)
            {
                TaskArgs.RemoveAll(
                    i =>
                        i.ChatId == chatId && i.Properties != null && i.Properties.ContainsKey("command") &&
                        i.Properties["command"] == command);
                SaveArgs();
            }
        }

    }
}