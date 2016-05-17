using System.Collections.Generic;
using Infrastructure;
using Logger;
using Telegram.Bot.Types;

namespace TelegramBot
{
    public class AuthBotCommand : IBotCommand
    {
        private readonly ILogger _logger;
        public string Name => "авторизоваться";

        public AuthBotCommand(ILogger logger)
        {
            _logger = logger;
        }

        public CommandExecuteResult Execute(string arg, IBot bot, string chatId, string user)
        {
            var words = arg.Split(' ');

            if (words.Length < 2)
                bot.SendTextMessage(chatId, "Для авторизации надо указать пароль.");

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

                _logger.Info("Ползьзователь " + user + " успешно авторизован");
                return new CommandExecuteResult("Вы успешно авторизованы! Для получения справки выполните команду /help.");
            }
            else
            {
                _logger.Warn("Неуспешная попытка авторизации пользователя " + user);
                return new CommandExecuteResult("Ошибка авторизации, вы ввели неправильный пароль.");
            }

        }

        public string GetHelp()
        {
            return
                "Команда 'авторизоваться пароль' - предоставляет досутп к функциям бота только тем у кого есть пароль.";
        }
    }
}