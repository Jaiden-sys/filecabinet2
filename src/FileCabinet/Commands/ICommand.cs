using System;
using System.Collections.Generic;
using System.Text;

namespace filecabinet.Commands
{
    public interface ICommand
    {
        string Name { get; }
        string HelpText { get; }
        public void Execute(string parameters);
    }
}
