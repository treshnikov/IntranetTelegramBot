using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using Infrastructure;
using Logger;
using Ninject;
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
        private ILogger _logger;
        private ICommandProcessor _commandProcessor;
        private IBotTaskProcessor _taskProcessor;
        private readonly IKernel _kernel;

        public TelegramBot()
        {
            System.IO.Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            _kernel = new StandardKernel();
            SetBindings();

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
            _bot = new Api(SettingsProvider.Get().BotApiKey)
            {
                IsReceiving = true
            };
            _bot.StartReceiving();
            _bot.MessageReceived += BotOnMessageReceived();

        }

        private void SetBindings()
        {
            _kernel.Bind<ILogger>().To<NLogger>();

            var commandTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => !p.IsInterface && typeof (IBotCommand).IsAssignableFrom(p)).ToList();

            var taskHandlerTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => !p.IsInterface && typeof (IBotTaskHandler).IsAssignableFrom(p)).ToList();

            foreach (var commandType in commandTypes)
            {
                _kernel.Bind<IBotCommand>().To(commandType).Named(commandType.Name);
            }

            foreach (var h in taskHandlerTypes)
            {
                _kernel.Bind<IBotTaskHandler>().To(h).Named(h.Name);
            }

            _kernel.Bind<ICommandProcessor>().To<CommandProcessor>()
                .InSingletonScope();

            _kernel.Bind<IBotTaskProcessor>().To<BotTaskProcessor>()
                .InSingletonScope()
                .WithConstructorArgument("bot", _bot);
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