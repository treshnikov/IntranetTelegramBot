using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Threading;
using Logger;
using Newtonsoft.Json;
using Ninject;
using Telegram.Bot;

namespace TelegramBot.Task
{
    public class BotTaskProcessor : IDisposable, IBotTaskProcessor
    {
        private static Api _bot;
        private readonly ILogger _logger;
        private List<IBotTaskHandler>  _handlers;
        public List<IBotTaskArg> TaskArgs { get; set; }
        private Dictionary<TimeSpan, Thread> Threads { get; set; } 
        private static readonly object TaskArgsLock = new object();

        public BotTaskProcessor(Api bot, ILogger logger)
        {
            TaskArgs = new List<IBotTaskArg>();
            Threads = new Dictionary<TimeSpan, Thread>();
            _handlers = new List<IBotTaskHandler>();

            _bot = bot;
            _logger = logger;
            LoadTasksArgs();
            CheckThreads();
        }

        public void SetHandlers(IEnumerable<IBotTaskHandler> handlers)
        {
            _handlers = new List<IBotTaskHandler>(handlers);
        }

        private void CheckThreads()
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
                            IEnumerable<IBotTaskArg> argsForPeriod;
                            lock (TaskArgsLock)
                            {
                                argsForPeriod = TaskArgs.Where(i => i.Period == taskArg.Period).ToArray();
                            }
                            if (!argsForPeriod.Any())
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
                                    _logger.Error("Ошибка обработки задачи " + ex );
                                }
                            }

                            Thread.Sleep(taskArg.Period);
                        }
                    });

                    Threads[taskArg.Period].Start();
                }
            }
        }

        public bool TryGetHandlerByName(string name, out IBotTaskHandler handler)
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

        private void LoadTasksArgs()
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

        public void SaveArgs()
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

        public void AddTaskArg(IBotTaskArg arg)
        {
            lock (TaskArgsLock)
            {
                TaskArgs.Add(arg);
                SaveArgs();
                CheckThreads();
            }
        }

        public void RemoveTaskArg(string chatId, string command)
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

        public void UpdateTaskArg(IBotTaskArg arg)
        {
            lock (TaskArgsLock)
            {
                var taskArg = TaskArgs.FirstOrDefault(i => i.Id == arg.Id);

                if (taskArg == null)
                    throw new ArgumentException("Не найден аргумент с id: " + arg.Id, "arg");

                taskArg.Period = arg.Period;
                taskArg.Properties = new Dictionary<string, string>(arg.Properties);
            }
        }

        public IBotTaskArg[] GetTaskArgs {
            get
            {
                lock (TaskArgsLock)
                {
                    return TaskArgs.ToArray();
                }
            } 
        }
    }
}