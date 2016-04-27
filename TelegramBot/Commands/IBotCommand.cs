using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot
{
    public interface IBotCommand
    {
        string Name { get; }
        CommandExecuteResult Execute(string arg, IBot bot, string chatId);
        string GetHelp();
    }

    public class CommandExecuteResult
    {
        public CommandResultType Type { get; set; }
        public string ResultAsText { get; set; }
        public string AdditionalInfo { get; set; }

        public CommandExecuteResult(string textResult)
        {
            ResultAsText = textResult;
            Type = CommandResultType.Text;
        }
    }

    public enum CommandResultType
    {
        Text
    }
}