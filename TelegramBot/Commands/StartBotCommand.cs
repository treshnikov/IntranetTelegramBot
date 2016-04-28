using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot
{
    public class StartBotCommand : IBotCommand
    {
        public string Name { get; }

        public StartBotCommand()
        {
            Name = "/start";
        }
        public CommandExecuteResult Execute(string words, IBot bot, string chatId, string user)
        {
            return new CommandExecuteResult("");
        }

        public string GetHelp()
        {
            return "";
        }
    }
}