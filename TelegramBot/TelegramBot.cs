using System;
using System.Linq;
using Infrastructure;
using Logger;
using Ninject;
using Telegram.Bot.Types;
using TelegramBot.Task;

namespace TelegramBot
{
    public class TelegramBot
    {
        private volatile bool _isRunning;
        private IntranetTelegramBot _bot;
        private readonly ILogger _logger;
        private readonly ICommandProcessor _commandProcessor;
        private readonly IBotTaskProcessor _taskProcessor;
        private readonly IKernel _kernel;

        public TelegramBot()
        {
            System.IO.Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            _kernel = new NinjectContainer();
            _logger = _kernel.Get<ILogger>();

            var botCommands = _kernel.GetAll<IBotCommand>().ToArray();
            var taskHandlers = _kernel.GetAll<IBotTaskHandler>().ToArray();

            _taskProcessor = _kernel.Get<IBotTaskProcessor>();
            _taskProcessor.SetHandlers(taskHandlers);

            _commandProcessor = _kernel.Get<ICommandProcessor>();
            _commandProcessor.SetCommands(botCommands);
        }

        public void Start()
        {
            if (_isRunning)
                return;

            _isRunning = true;

            _bot = (IntranetTelegramBot) _kernel.Get<IBot>();

            _bot.IsReceiving = true;
            _bot.StartReceiving();
            _bot.MessageReceived += BotOnMessageReceived();
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
                    _logger.Error("Ошибка обработки команды + " + args + " " + ex);
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