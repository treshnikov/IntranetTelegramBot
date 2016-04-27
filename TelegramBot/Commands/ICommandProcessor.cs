using System.Collections.Generic;
using Telegram.Bot;

namespace TelegramBot
{
    public interface ICommandProcessor
    {
        void SetCommands(IEnumerable<IBotCommand> commands);
        bool TryGetCommandByName(string name, out IBotCommand cmd);
        bool TryExecuteCommand(string arg, IBot bot, string chatId, out CommandExecuteResult result);
    }
}