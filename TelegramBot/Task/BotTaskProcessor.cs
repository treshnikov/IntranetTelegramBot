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
        private static Dictionary<TimeSpan, Thread> Threads { get; set; } 
        private static object TaskArgsLock = new object();

        static BotTaskProcessor()
        {
            TaskArgs = new List<IBotTaskArg>();
            Threads = new Dictionary<TimeSpan, Thread>();
        }

        public BotTaskProcessor(Api bot, ILogger logger)
        {
            _bot = bot;
            _logger = logger;
            PrepareHandlers();
            LoadTasksArgs();
            CheckThreads();
        }

        private static void CheckThreads()
        {
            lock (TaskArgsLock)
            {

                foreach (var taskArg in TaskArgs)
                {
                    if (Threads.ContainsKey(taskArg.Period))
                        continue;

                    Threads[taskArg.Period] = new Thread(() =>
                    {
                        while (true)
                        {
                            IBotTaskArg[] argsForPeriod;
                            lock (TaskArgsLock)
                            {
                                argsForPeriod = TaskArgs.Where(i => i.Period == taskArg.Period).ToArray();
                            }
                            if (argsForPeriod.Length == 0)
                                break;

                            foreach (var arg in argsForPeriod)
                            {
                                IBotTaskHandler taskhandler;
                                if (!TryGetHandlerByName(arg.TaskHandlerName, out taskhandler))
                                    continue;

                                try
                                {
                                    taskhandler.Handle(_bot, arg);
                                }
                                catch (Exception ex)
                                {
                                    //todo
                                }
                            }

                            Thread.Sleep(taskArg.Period);
                        }
                    });

                    Threads[taskArg.Period].Start();
                }
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

        private static void LoadTasksArgs()
        {
            lock (TaskArgsLock)
            {
                if (File.Exists("tasks.json"))
                {
                    TaskArgs =
                        new List<IBotTaskArg>(JsonConvert.DeserializeObject<BotTaskArg[]>(File.ReadAllText("tasks.json")));
                }
            }
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
                SaveArgs();
                CheckThreads();
            }
        }

        public static void RemoveTaskArg(string chatId, string command)
        {
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