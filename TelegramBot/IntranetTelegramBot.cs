using System.Net;
using System.Net.Http;
using Telegram.Bot;

namespace TelegramBot
{
    public class IntranetTelegramBot : TelegramBotClient, IBot
    {
        public void SendTextMessage(string chatId, string text)
        {
            base.SendTextMessageAsync(chatId, text);
        }

        public IntranetTelegramBot(string token) : base(token)
        {
        }

        public IntranetTelegramBot(string token, IWebProxy webProxy) : base(token, webProxy)
        {
        }

        public IntranetTelegramBot(string token, HttpClient client) : base(token, client)
        {
        }


    }
}