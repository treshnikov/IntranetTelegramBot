using System.Linq;
using Telegram.Bot;
using TelegramBot.Task;

namespace TelegramBot
{
    public class GetSubscribtionListBotCommand : IBotCommand
    {
        private readonly IBotTaskProcessor _taskProcessor;
        public string Name { get; }

        public GetSubscribtionListBotCommand(IBotTaskProcessor taskProcessor)
        {
            _taskProcessor = taskProcessor;
            Name = "подписки";
        }

        public CommandExecuteResult Execute(string arg, Api bot, string chatId)
        {
            var subscribtion = _taskProcessor.GetTaskArgs.Where(i => i.ChatId == chatId).ToArray();

            if (subscribtion.Length == 0)
            {
                return new CommandExecuteResult("У вас нет пописок");
            }

            var res = "";
            var idx = 1;
            foreach (var s in subscribtion)
            {
                res += idx + " - " + s.Properties["command"] + "\r\n";
                idx++;
            }

            return new CommandExecuteResult(res);
        }

        public string GetHelp()
        {
            return "Команда 'подписки' показывает список текущих подписок";
        }
    }
}