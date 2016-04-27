using System;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Task;

namespace TelegramBot
{
    public class WatchToCommand : IBotCommand
    {
        private readonly ICommandProcessor _commandProcessor;
        private readonly IBotTaskProcessor _taskProcessor;
        public string Name { get; }

        public WatchToCommand(ICommandProcessor commandProcessor, IBotTaskProcessor taskProcessor)
        {
            _commandProcessor = commandProcessor;
            _taskProcessor = taskProcessor;
            Name = "�����������";
        }

        public CommandExecuteResult Execute(string arg, Api bot, string chatId)
        {
            var words = arg.Split(' ');

            if (words.Length < 2)
            {
                bot.SendTextMessage(chatId, "�� ������� ���������� ��������� �������. ������ ������� - '����������� ��� ������'");
                return new CommandExecuteResult("");
            }

            var command = arg.Substring(Name.Length + 1);
            IBotCommand cmdHandler;
            if (!_commandProcessor.TryGetCommandByName(command.Split(' ')[0], out cmdHandler))
            {
                bot.SendTextMessage(chatId, "����������� ������� " + command.Split(' ')[0]);
                return new CommandExecuteResult("");
            }

            var taskArg = new BotTaskArg()
            {
                Id = Guid.NewGuid(),
                ChatId = chatId,
                Period = TimeSpan.FromSeconds(30),
                Properties = new Dictionary<string, string>(),
                TaskHandlerName = "�����������",
            };

            taskArg.Properties["command"] = command;

            _taskProcessor.AddTaskArg(taskArg);

            var res = new GetSubscribtionListBotCommand(_taskProcessor).Execute("��������", bot, chatId).ResultAsText;
            return new CommandExecuteResult("�������� ������� ���������:\r\n" + res);
        }

        public string GetHelp()
        {
            return "������� '�����������' ��������� �������� �� ��������� ��������� �������, �������� '����������� ��� ������'";
        }
    }

    


}