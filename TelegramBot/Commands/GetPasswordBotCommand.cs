using Infrastructure;

namespace TelegramBot
{
    public class GetPasswordBotCommand : IBotCommand
    {
        public string Name => "������";
        public CommandExecuteResult Execute(string arg, IBot bot, string chatId, string user)
        {
            if (user != "treshnikov")
                return new CommandExecuteResult("������������ ����.");

            return new CommandExecuteResult(SettingsProvider.Get().Password);
        }

        public string GetHelp()
        {
            return "";
        }
    }
}