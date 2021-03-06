using System;
using System.Linq;
using System.Net;
using Infrastructure;
using Logger;
using Ninject;
using Ninject.Modules;
using TelegramBot.Task;

namespace TelegramBot
{
    public class NinjectContainer : StandardKernel
    {
        public NinjectContainer()
        {
            SetBindings();
        }

        private void SetBindings()
        {
            Bind<ILogger>().To<NLogger>().InSingletonScope();

            var botSettings = Bind<IBot>()
                .To<IntranetTelegramBot>()
                .InSingletonScope()
                .WithConstructorArgument("token", SettingsProvider.Get().BotApiKey);
            if (!string.IsNullOrWhiteSpace(SettingsProvider.Get().Proxy))
            {
                var proxy = SettingsProvider.Get().Proxy;
                botSettings.WithConstructorArgument("webProxy", new WebProxy(proxy));
            }

            var commandTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => !p.IsInterface && typeof(IBotCommand).IsAssignableFrom(p)).ToList();

            var taskHandlerTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => !p.IsInterface && typeof(IBotTaskHandler).IsAssignableFrom(p)).ToList();

            foreach (var commandType in commandTypes)
            {
                Bind<IBotCommand>().To(commandType).Named(commandType.Name);
            }

            foreach (var h in taskHandlerTypes)
            {
                Bind<IBotTaskHandler>().To(h).Named(h.Name);
            }

            Bind<ICommandProcessor>().To<CommandProcessor>()
                .InSingletonScope();

            Bind<IBotTaskProcessor>().To<BotTaskProcessor>()
                .InSingletonScope();
        }

    }
}