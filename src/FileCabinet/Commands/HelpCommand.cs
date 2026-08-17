using System;
using System.Collections.Generic;
using System.Text;
using filecabinet;
namespace filecabinet.Commands
{
    public class HelpCommand : ICommand
    {
        public string Name => "help";
        public string HelpText => "Shows all help commands";
        private readonly IReadOnlyList<ICommand> _commands;
        public HelpCommand(IReadOnlyList<ICommand> commands)
        {  _commands = commands; }
        public void Execute(string parameters) 
        {
            foreach (var command in _commands)
            {
                Console.WriteLine($"{command.Name}\t- {command.HelpText}");
            }
        }
    }
}
