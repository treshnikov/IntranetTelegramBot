using Telegram.Bot;

namespace TelegramBot
{
    public interface IBot
    {
        void SendTextMessage(string chatId, string text);
    }

}