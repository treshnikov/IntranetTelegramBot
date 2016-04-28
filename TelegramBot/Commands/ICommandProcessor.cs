using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot
{
    public interface ICommandProcessor
    {
        void SetCommands(IEnumerable<IBotCommand> commands);
        bool TryGetCommandByName(string name, out IBotCommand cmd);
        bool TryExecuteCommand(string arg, IBot bot, string chatId, string user, out CommandExecuteResult result);
        IBotCommand[] Commands { get; }
    }
}