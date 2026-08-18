using System;
using System.Collections.Generic;
using System.Text;

namespace filecabinet.Commands
{
    public class ListCommand :  ICommand
    {
        public string Name => "list";
        public string HelpText => "Show list of created records";
        private readonly IFileCabinetService _service;
        public ListCommand(IFileCabinetService service) 
        {
            _service = service;
        }
        public void Execute(string parameters)
        {
            var records = _service.GetRecords();

            foreach (var record in records)
            {
                Console.WriteLine($"#{record.Id},{record.FirstName},{record.LastName},{record.DateOfBirth},{record.ArchiveId},{record.Weight},{record.Type}");
            }
        }
    }
}
