using System.Linq;
using Infrastructure;

namespace TelegramBot
{
    public class GetAllowedUsersBotCommand : IBotCommand
    {
        public string Name => "пользователи";
        public CommandExecuteResult Execute(string arg, IBot bot, string chatId, string user)
        {
            var users = SettingsProvider.Get().AllowedUsers;
            if (users == null)
                return new CommandExecuteResult("Пользователей не зарегистрировано");

            return new CommandExecuteResult(users.Aggregate((a, b) => b + a + ", "));
        }

        public string GetHelp()
        {
            return "";
        }
    }
}