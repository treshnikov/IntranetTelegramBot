using System;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Task;

namespace TelegramBot
{
    public class WatchToCommand : IBotCommand
    {
        public string Name { get; }

        public WatchToCommand()
        {
            Name = "�����������";
        }

        public CommandExecuteResult Execute(string arg, Api bot, string chatId)
        {
            var words = arg.Split(' ');

            if (words.Length < 2)
            {
                bot.SendTextMessage(chatId, "�� ������� ���������� ��������� �������. ������ ������� - '����������� ��� ������'");
            }

            var taskArg = new BotTaskArg()
            {
                ChatId = chatId,
                Period = TimeSpan.FromSeconds(30),
                Properties = new Dictionary<string, string>(),
                TaskHandlerName = "�����������",
            };
            taskArg.Properties["command"] = arg.Substring(Name.Length + 1);

            BotTaskProcessor.AddTaskArg(taskArg);

            var res = new GetSubscribtionListBotCommand().Execute("��������", bot, chatId).ResultAsText;
            return new CommandExecuteResult("�������� ������� ���������:\r\n" + res);
        }

        public string GetHelp()
        {
            return "������� '�����������' ��������� �������� �� ��������� ��������� �������, �������� '����������� ��� ������'";
        }
    }

    


}