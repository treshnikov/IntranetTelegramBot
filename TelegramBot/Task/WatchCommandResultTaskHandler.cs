using System;
using System.Collections.Generic;
using Logger;
using Telegram.Bot;

namespace TelegramBot.Task
{
    public class WatchCommandResultTaskHandler : IBotTaskHandler
    {
        private readonly IBotTaskProcessor _taskProcessor;
        private readonly ICommandProcessor _commandProcessor;
        public string Name { get; }

        public WatchCommandResultTaskHandler(IBotTaskProcessor taskProcessor, ICommandProcessor commandProcessor)
        {
            _taskProcessor = taskProcessor;
            _commandProcessor = commandProcessor;
            Name = "�����������";
        }

        public void Handle(IBot bot, IBotTaskArg botTaskArg)
        {
            var command = botTaskArg.Properties["command"];

            CommandExecuteResult res;
            if (_commandProcessor.TryExecuteCommand(command, bot, botTaskArg.ChatId.ToString(), botTaskArg.User, out res))
            {
                var currentValue = botTaskArg.Properties.ContainsKey("currentValue")
                    ? botTaskArg.Properties["currentValue"]
                    : "";

                var compareFunc = command.StartsWith("���")
                    ? new OfficeTimeOldNewValueComparer().Func
                    : (o, n) => o == n;

                if (!compareFunc(currentValue, res.ResultAsText))
                {
                    botTaskArg.Properties["currentValue"] = res.ResultAsText;
                    _taskProcessor.UpdateTaskArg(botTaskArg);

                    if (!string.IsNullOrWhiteSpace(currentValue))
                    {
                        bot.SendTextMessage(botTaskArg.ChatId, "���������� ��� '" + command + "': " + res.ResultAsText);
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
                      ������ ������:
                         ����� ���� �������� ��� � ����� ������, ������ 2 ������  �����
                         ����� ���� �������� � ����� ������, ��������������� ����� 18 �����  �����

                      ������� ���������:
                         ������� �� ����� ��������� - �� '����� �����' �� ����� '�����'
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
                    var wordPos = str.IndexOf("�����");
                    str = str.Remove(numberIdx, wordPos - numberIdx + 5);
                    numberIdx = str.IndexOfAny("0123456789".ToCharArray());
                }
                return str;
            }
        }
    }
}