using System.ServiceProcess;

namespace TelegramBotService
{
    public partial class TelegramBotIntranet : ServiceBase
    {
        TelegramBot.TelegramBot _bot; 

        public TelegramBotIntranet()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _bot = new TelegramBot.TelegramBot();
            _bot.Start();
        }

        protected override void OnStop()
        {
            _bot.Stop();
        }
    }
}
