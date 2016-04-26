using System;
using System.Linq;
using Telegram.Bot;
using TelegramBot.Task;

namespace TelegramBot
{
    public class UnsubscribeBotCommand : IBotCommand
    {
        public string Name { get; }

        public UnsubscribeBotCommand()
        {
            Name = "����������";
        }

        public CommandExecuteResult Execute(string arg, Api bot, string chatId)
        {
            var words = arg.Split(' ');

            if (words.Length < 2)
            {
                bot.SendTextMessage(chatId, "������� ���������� ����� ��������, �������� '���������� 3'");
            }
            var subscribtionNumber = Int32.Parse(words[1]);

            var subscribtion = BotTaskProcessor.TaskArgs.Where(i => i.ChatId == chatId).ToArray();

            if (subscribtion.Length < subscribtionNumber)
            {
                throw new ArgumentException();
            }

            BotTaskProcessor.RemoveTaskArg(chatId, subscribtion[subscribtionNumber - 1].Properties["command"]);

            var res = new GetSubscribtionListBotCommand().Execute("��������", bot, chatId).ResultAsText;
            return new CommandExecuteResult("�������� ������� �������:\r\n"+res);
        }

        public string GetHelp()
        {
            return "������� '����������' ������� �������� �� ����������� ������, �������� '���������� 3'. ����� ������ ������ ������� ��������� ������� '��������'.";
        }
    }
}