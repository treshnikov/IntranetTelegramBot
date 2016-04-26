using Telegram.Bot;

namespace TelegramBot.Task
{
    public interface IBotTaskHandler
    {
        string Name { get; }
        void Handle(Api bot, IBotTaskArg botTaskArg);
    }
}