using System.Linq;
using Infrastructure;
using Telegram.Bot;

namespace TelegramBot
{
    public class GetUserLocationCommand : IBotCommand
    {
        public string Name { get; }

        public GetUserLocationCommand()
        {
            Name = "где";
        }

        public CommandExecuteResult Execute(string arg, Api bot, string chatId)
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
                return new CommandExecuteResult("Данных для \"" + userName + "\"  не обнаружено");
            }
            else
            {
                var res = "";
                foreach (var user in users)
                {
                    var info = Intranet.GetUserInOfficeInfo(user.email);
                    res += 
                        user.fullname + " " + info.inOfficeRus.ToLower() + " " + info.office + " " +
                        (info.inOfficeRus == "В офисе" ? "вошел " + info.timeAgoText : info.timeAgoText) + "\r\n ";

                }

                return new CommandExecuteResult(res);
            }
        }

        public string GetHelp()
        {
            return "Команда 'где' показывает местоположение пользователя из СКУД, например 'где иванов'";
        }
    }
}