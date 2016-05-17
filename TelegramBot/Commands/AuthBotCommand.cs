using System.Collections.Generic;
using Infrastructure;
using Logger;
using Telegram.Bot.Types;

namespace TelegramBot
{
    public class AuthBotCommand : IBotCommand
    {
        private readonly ILogger _logger;
        public string Name => "��������������";

        public AuthBotCommand(ILogger logger)
        {
            _logger = logger;
        }

        public CommandExecuteResult Execute(string arg, IBot bot, string chatId, string user)
        {
            var words = arg.Split(' ');

            if (words.Length < 2)
                bot.SendTextMessage(chatId, "��� ����������� ���� ������� ������.");

            var userPassword = words[1];
            var settings = SettingsProvider.Get();
            var password = settings.Password;
            if (userPassword == password)
            {
                if (settings.AuthChatIds == null)
                    settings.AuthChatIds = new string[0] {};

                var newUsersList = new List<string>(settings.AuthChatIds) {chatId};
                settings.AuthChatIds = newUsersList.ToArray();

                SettingsProvider.Set(settings);

                _logger.Info("������������� " + user + " ������� �����������");
                return new CommandExecuteResult("�� ������� ������������! ��� ��������� ������� ��������� ������� /help.");
            }
            else
            {
                _logger.Warn("���������� ������� ����������� ������������ " + user);
                return new CommandExecuteResult("������ �����������, �� ����� ������������ ������.");
            }

        }

        public string GetHelp()
        {
            return
                "������� '�������������� ������' - ������������� ������ � �������� ���� ������ ��� � ���� ���� ������.";
        }
    }
}