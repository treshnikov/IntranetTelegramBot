using System.Linq;
using Infrastructure;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot
{
    public class GetUserPhoneCommand : IBotCommand
    {
        public string Name { get; }

        public GetUserPhoneCommand()
        {
            Name = "�������";
        }

        public CommandExecuteResult Execute(string arg, Api bot, string chatId)
        {
            var words = arg.Split(' ');

            if (words.Length < 2)
                bot.SendTextMessage(chatId, "�� ������� ��� ������������");

            var allUsers = Intranet.GetUsers();
            var userName = arg.Substring(Name.Length + 1);

            var users =
                allUsers.Where(i => i.fullname.ToLower().Contains(userName.ToLower())).ToList();

            if (users.Count == 0)
            {
                return new CommandExecuteResult(
                    "������ ��� \"" + userName + "\"  �� ����������");
            }
            else
            {
                var res = "";
                foreach (var user in users)
                {
                    res += user.fullname + " " + user.mobilephone + " " + user.workphone + "\r\n ";
                }

                return new CommandExecuteResult(res);
            }
        }

        public string GetHelp()
        {
            return "������� '�������' ���������� ������ ��������� ���������� � ����� ��������, �������� '������� ������'";
        }
    }
}