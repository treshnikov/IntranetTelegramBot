using System.Linq;
using Telegram.Bot;
using TelegramBot.Task;

namespace TelegramBot
{
    public class GetSubscribtionListBotCommand : IBotCommand
    {
        public string Name { get; }

        public GetSubscribtionListBotCommand()
        {
            Name = "подписки";
        }

        public CommandExecuteResult Execute(string arg, Api bot, string chatId)
        {
            var subscribtion = BotTaskProcessor.TaskArgs.Where(i => i.ChatId == chatId).ToArray();

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