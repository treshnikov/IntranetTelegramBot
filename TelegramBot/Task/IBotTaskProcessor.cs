using System;
using System.Collections.Generic;

namespace TelegramBot.Task
{
    public interface IBotTaskProcessor : IDisposable
    {
        void SetHandlers(IEnumerable<IBotTaskHandler> handlers);
        void AddTaskArg(IBotTaskArg arg);
        void RemoveTaskArg(string chatId, string command);
        void UpdateTaskArg(IBotTaskArg arg);
        IBotTaskArg[] GetTaskArgs { get; }
    }
}