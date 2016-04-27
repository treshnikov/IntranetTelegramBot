using Telegram.Bot;

namespace TelegramBot
{
    public class IntranetTelegramBot : Api, IBot
    {
        public void SendTextMessage(string chatId, string text)
        {
            base.SendTextMessage(chatId, text);
        }

        public IntranetTelegramBot(string token) : base(token)
        {
        }
    }
}