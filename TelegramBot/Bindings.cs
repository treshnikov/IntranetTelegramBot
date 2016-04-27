using System.Runtime.InteropServices;
using Logger;
using Ninject.Modules;
using TelegramBot;
using TelegramBot.Task;

namespace Infrastructure
{
    public class Bindings : NinjectModule
    {
        public override void Load()
        {
            Bind<ILogger, NLogger>();
        }
    }
}