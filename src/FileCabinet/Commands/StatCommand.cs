using System;
using System.Collections.Generic;
using System.Text;

namespace filecabinet.Commands
{
    public class StatCommand : ICommand
    {
        public string Name => "stat";
        public string HelpText => "Shows number of records";
        private readonly IFileCabinetService _service;
        public StatCommand(IFileCabinetService service)
        {
            this._service = service;
        }
        public void Execute(string parameters)
        {
            var recordsCount = _service.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }
    }
}
