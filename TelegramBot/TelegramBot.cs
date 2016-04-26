using System;
using System.Threading;
using Infrastructure;
using Logger;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Task;
using File = System.IO.File;

namespace TelegramBot
{
    public class TelegramBot
    {
        private volatile bool _isRunning = false;
        private Api _bot;
        private NLogger _logger;
        private CommandProcessor _commandProcessor;
        private BotTaskProcessor _taskProcessor;

        public TelegramBot()
        {
            System.IO.Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            _logger = new NLogger();
        }

        public void Start()
        {
            if (_isRunning)
                return;

            _isRunning = true;
            _bot = new Api(SettingsProvider.Get().BotApiKey)
            {
                IsReceiving = true
            };
            _bot.StartReceiving();
            _bot.MessageReceived += BotOnMessageReceived();

            _commandProcessor = new CommandProcessor();
            _taskProcessor = new BotTaskProcessor(_bot, _logger);

        }

        private EventHandler<MessageEventArgs> BotOnMessageReceived()
        {
            return (sender, arg) =>
            {
                var args = arg.Message.Text;
                try
                {
                    var contact = arg.Message.From.FirstName + " " + arg.Message.From.LastName;
                    var log = "chatId: " + arg.Message.Chat.Id + 
                              " username: " + arg.Message.From.Username +
                              " contact: " + contact + 
                              " text: " + args;
                    
                    _logger.Debug(log);

                    CommandExecuteResult res;
                    _commandProcessor.TryExecuteCommand(args, _bot, arg.Message.Chat.Id.ToString(), out res);

                    if (res.Type == CommandResultType.Text)
                    {
                        _bot.SendTextMessage(arg.Message.Chat.Id, res.ResultAsText);
                    }

                }
                catch (Exception ex)
                {
                    _bot.SendTextMessage(arg.Message.Chat.Id, "Не удалось выполнить команду \"" + args + "\"");
                }

            };
        }

        public void Stop()
        {
            if (!_isRunning)
                return;

            _bot.StopReceiving();
            _bot.MessageReceived -= BotOnMessageReceived();
            _isRunning = false;

            _taskProcessor.Dispose();
        }       
    }
}