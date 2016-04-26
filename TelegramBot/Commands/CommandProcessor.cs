using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot
{
    public class CommandProcessor
    {
        private readonly List<IBotCommand> _commands;

        public CommandProcessor()
        {
            _commands = new List<IBotCommand>();

            var commandType = typeof(IBotCommand);
            var commandTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => !p.IsInterface && commandType.IsAssignableFrom(p)).ToList();

            foreach (var ct in commandTypes)
            {
                var cmd = Activator.CreateInstance(ct);
                _commands.Add((IBotCommand)cmd);
            }

        }

        public bool TryGetCommandByName(string name, out IBotCommand cmd)
        {
            foreach (var command in _commands)
            {
                if (string.Equals(command.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    cmd = command;
                    return true;
                }
            }

            cmd = null;
            return false;
        }

        public bool TryExecuteCommand(string arg, Api bot, string chatId, out CommandExecuteResult result)
        {
            var words = arg.Split(' ');

            if (words == null || words.Length <= 0)
            {
                result = null;
                return false;
            }

            IBotCommand cmd;
            var cmdFound = TryGetCommandByName(words[0], out cmd);

            if (cmdFound)
            {
                result = cmd.Execute(arg, bot, chatId);
                return true;
            }
            else
            {
                result = null;
                bot.SendTextMessage(chatId, "Неизвестная команда \"" + words[0] + "\"");
                return false;
            }

        }
    }
}