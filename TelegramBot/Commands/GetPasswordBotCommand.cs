using Infrastructure;

namespace TelegramBot
{
    public class GetPasswordBotCommand : IBotCommand
    {
        public string Name => "пароль";
        public CommandExecuteResult Execute(string arg, IBot bot, string chatId, string user)
        {
            if (user != "treshnikov")
                return new CommandExecuteResult("Недостаточно прав.");

            return new CommandExecuteResult(SettingsProvider.Get().Password);
        }

        public string GetHelp()
        {
            return "";
        }
    }
}