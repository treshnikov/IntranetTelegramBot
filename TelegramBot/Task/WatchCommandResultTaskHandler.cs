using System;
using System.Collections.Generic;
using Logger;
using Telegram.Bot;

namespace TelegramBot.Task
{
    public class WatchCommandResultTaskHandler : IBotTaskHandler
    {
        public string Name { get; }

        public WatchCommandResultTaskHandler()
        {
            Name = "подписаться";
        }

        public void Handle(Api bot, IBotTaskArg botTaskArg)
        {
            var command = botTaskArg.Properties["command"];

            var cmdProc = new CommandProcessor();
            CommandExecuteResult res;
            if (cmdProc.TryExecuteCommand(command, bot, botTaskArg.ChatId.ToString(), out res))
            {
                var currentValue = botTaskArg.Properties.ContainsKey("currentValue")
                    ? botTaskArg.Properties["currentValue"]
                    : "";

                var compareFunc = command.StartsWith("где")
                    ? new OfficeTimeOldNewValueComparer().Func
                    : (o, n) => o == n;

                if (!compareFunc(currentValue, res.ResultAsText))
                {
                    botTaskArg.Properties["currentValue"] = res.ResultAsText;
                    BotTaskProcessor.SaveArgs();

                    if (!string.IsNullOrWhiteSpace(currentValue))
                    {
                        bot.SendTextMessage(botTaskArg.ChatId, "Оповещение для '" + command + "': " + res.ResultAsText);
                    }
                }
            }
        }


        class OfficeTimeOldNewValueComparer
        {
            public Func<string, string, bool> Func { get; set; }

            public OfficeTimeOldNewValueComparer()
            {

                Func = (oldVal, newVal) =>
                {
                    /*
                      Пример данных:
                         Ивано Иван Иванович был в офисе Самара, Кирова 2 месяца  назад
                         Ивано Иван Иванович в офисе Самара, Галактионовская вошел 18 минут  назад

                      Правило сравнения:
                         Удалить из строк вхождения - от 'перой цифры' до слова 'назад'
                    */

                    var a = PrepareString(oldVal);
                    var b = PrepareString(newVal);

                    a = a.Replace(" ", "");
                    b = b.Replace(" ", "");
                    
                    return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
                };
            }

            private string PrepareString(string arg)
            {
                var str = arg;
                var numberIdx = str.IndexOfAny("0123456789".ToCharArray());
                while (numberIdx >= 0)
                {
                    var wordPos = str.IndexOf("назад");
                    str = str.Remove(numberIdx, wordPos - numberIdx + 5);
                    numberIdx = str.IndexOfAny("0123456789".ToCharArray());
                }
                return str;
            }
        }
    }
}