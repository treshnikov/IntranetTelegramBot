using Telegram.Bot;

namespace TelegramBot
{
    public class StartBotCommand : IBotCommand
    {
        public string Name { get; }

        public StartBotCommand()
        {
            Name = "/start";
        }
        public CommandExecuteResult Execute(string words, Api bot, string chatId)
        {
            return new CommandExecuteResult("");
        }

        public string GetHelp()
        {
            return "";
        }
    }
}