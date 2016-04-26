using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SimpleImpersonation;

namespace TelegramBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var b = new TelegramBot();
            Console.WriteLine("TelegramBot.Intranet");
            b.Start();
            Console.WriteLine("Press any key to stop bot...");
            Console.ReadLine();
            b.Stop();
        }


    }
}
