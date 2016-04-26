using System;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot
{
    public class GetHelpCommand : IBotCommand
    {
        public GetHelpCommand()
        {
            Name = "/help";
        }

        public string Name { get; }
        public CommandExecuteResult Execute(string arg, Api bot, string chatId)
        {
            var res = "";

            var commandType = typeof(IBotCommand);
            var commandTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => !p.IsInterface && commandType.IsAssignableFrom(p)).ToList();

            foreach (var ct in commandTypes)
            {
                var cmd = (IBotCommand)Activator.CreateInstance(ct);
                if (cmd.GetHelp() != "")
                    res += "\u27A1 " + cmd.GetHelp() + "\r\n";
            }

            return new CommandExecuteResult(res);
        }

        public string GetHelp()
        {
            return "";
        }
    }
}