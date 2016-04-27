using Telegram.Bot;

namespace TelegramBot.Task
{
    public interface IBotTaskHandler
    {
        string Name { get; }
        void Handle(IBot bot, IBotTaskArg botTaskArg);
    }
}