using Telegram.Bot;

namespace TelegramBot
{
    public interface IBot
    {
        void SendTextMessage(string chatId, string text);
    }

    public class Bot : IBot
    {
        private readonly Api _bot;

        public Bot(Api bot)
        {
            _bot = bot;
        }

        public void SendTextMessage(string chatId, string text)
        {
            _bot.SendTextMessage(chatId, text);
        }
    }
}