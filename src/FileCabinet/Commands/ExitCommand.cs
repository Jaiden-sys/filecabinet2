using System;
using System.Collections.Generic;
using System.Text;

namespace filecabinet.Commands
{
    public class ExitCommand : ICommand
    {
        public string Name => "exit";
        public string HelpText => "Exits the application";
        public void Execute(string parameters)
        {
            Environment.Exit(0);
        }
    }
}
