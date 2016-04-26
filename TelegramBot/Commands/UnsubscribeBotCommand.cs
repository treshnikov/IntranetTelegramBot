using System;
using System.Linq;
using Telegram.Bot;
using TelegramBot.Task;

namespace TelegramBot
{
    public class UnsubscribeBotCommand : IBotCommand
    {
        public string Name { get; }

        public UnsubscribeBotCommand()
        {
            Name = "отписаться";
        }

        public CommandExecuteResult Execute(string arg, Api bot, string chatId)
        {
            var words = arg.Split(' ');

            if (words.Length < 2)
            {
                bot.SendTextMessage(chatId, "Укажите порядковый номер подписки, например 'отписаться 3'");
            }
            var subscribtionNumber = Int32.Parse(words[1]);

            var subscribtion = BotTaskProcessor.TaskArgs.Where(i => i.ChatId == chatId).ToArray();

            if (subscribtion.Length < subscribtionNumber)
            {
                throw new ArgumentException();
            }

            BotTaskProcessor.RemoveTaskArg(chatId, subscribtion[subscribtionNumber - 1].Properties["command"]);

            var res = new GetSubscribtionListBotCommand().Execute("подписки", bot, chatId).ResultAsText;
            return new CommandExecuteResult("Подписка успешна удалена:\r\n"+res);
        }

        public string GetHelp()
        {
            return "Команда 'отписаться' удаляет подписку по порядковому номеру, например 'отписаться 3'. Чтобы узнать номера подисок выполните команду 'подписки'.";
        }
    }
}