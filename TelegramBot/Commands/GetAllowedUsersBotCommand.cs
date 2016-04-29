using System.Linq;
using Infrastructure;

namespace TelegramBot
{
    public class GetAllowedUsersBotCommand : IBotCommand
    {
        public string Name => "������������";
        public CommandExecuteResult Execute(string arg, IBot bot, string chatId, string user)
        {
            var users = SettingsProvider.Get().AllowedUsers;
            if (users == null)
                return new CommandExecuteResult("������������� �� ����������������");

            return new CommandExecuteResult(users.Aggregate((a, b) => b + a + ", "));
        }

        public string GetHelp()
        {
            return "";
        }
    }
}