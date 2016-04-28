namespace TelegramBot
{
    public class GetHelpCommand : IBotCommand
    {
        private readonly ICommandProcessor _commandProcessor;

        public GetHelpCommand(ICommandProcessor commandProcessor)
        {
            _commandProcessor = commandProcessor;
            Name = "/help";
        }

        public string Name { get; }
        public CommandExecuteResult Execute(string arg, IBot bot, string chatId, string user)
        {
            var res = "";


            foreach (var cmd in _commandProcessor.Commands)
            {
                if (cmd.GetHelp() != "")
                    res += "\u27A1 " + cmd.GetHelp() + "\r\n";

            }

            return new CommandExecuteResult(res);
        }

        public string GetHelp()
        {
            return "";
        }
    }
}