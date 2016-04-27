using System;
using System.Linq;
using Telegram.Bot;
using TelegramBot.Task;

namespace TelegramBot
{
    public class UnsubscribeBotCommand : IBotCommand
    {
        private readonly IBotTaskProcessor _taskProcessor;
        public string Name { get; }

        public UnsubscribeBotCommand(IBotTaskProcessor taskProcessor)
        {
            _taskProcessor = taskProcessor;
            Name = "����������";
        }

        public CommandExecuteResult Execute(string arg, IBot bot, string chatId)
        {
            var words = arg.Split(' ');

            if (words.Length < 2)
            {
                bot.SendTextMessage(chatId, "������� ���������� ����� ��������, �������� '���������� 3'");
            }
            var subscribtionNumber = Int32.Parse(words[1]);

            var subscribtion = _taskProcessor.GetTaskArgs.Where(i => i.ChatId == chatId).ToArray();

            if (subscribtion.Length < subscribtionNumber)
            {
                throw new ArgumentException();
            }

            _taskProcessor.RemoveTaskArg(chatId, subscribtion[subscribtionNumber - 1].Properties["command"]);

            var res = new GetSubscribtionListBotCommand(_taskProcessor).Execute("��������", bot, chatId).ResultAsText;
            return new CommandExecuteResult("�������� ������� �������:\r\n"+res);
        }

        public string GetHelp()
        {
            return "������� '����������' ������� �������� �� ����������� ������, �������� '���������� 3'. ����� ������ ������ ������� ��������� ������� '��������'.";
        }
    }
}