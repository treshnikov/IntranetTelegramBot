using System.Linq;
using Infrastructure;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot
{
    public class GetUserMailCommand : IBotCommand
    {
        public string Name { get; }

        public GetUserMailCommand()
        {
            Name = "почта";
        }

        public CommandExecuteResult Execute(string arg, IBot bot, string chatId)
        {
            var words = arg.Split(' ');

            if (words.Length < 2)
                bot.SendTextMessage(chatId, "не указано имя пользователя");

            var allUsers = Intranet.GetUsers();
            var userName = arg.Substring(Name.Length + 1);
            var users =
                allUsers.Where(i => i.fullname.ToLower().Contains(userName.ToLower())).ToList();

            if (users.Count == 0)
            {
                return new CommandExecuteResult(
                    "Почта для \"" + userName + "\"  не обнаружена");
            }
            else
            {
                var res = "";
                foreach (var user in users)
                {
                    res += user.fullname + " " + user.email + "\r\n ";
                }

                return new CommandExecuteResult(res);
            }
        }

        public string GetHelp()
        {
            return "Команда 'почта' показывает почтовый адрес сотрудника с сайта интранет, например 'почта иванов'";
        }
    }
}