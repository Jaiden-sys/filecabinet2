using filecabinet.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace filecabinet.Commands
{
    public class RestoreCommand : ICommand
    {
        public string Name => "restore";
        public string HelpText => "Restores selected command";
        private readonly IRestorable _restorable;
        private readonly IFileCabinetService _service;
        public RestoreCommand(IRestorable restorable, IFileCabinetService service) 
        { 
            _restorable = restorable;
            _service = service;
        }
        public void Execute(string parameters)
        {
            if (_restorable is null || !int.TryParse(parameters, out int id))
            {
                Console.WriteLine("Invalid id.");
                return;
            }
            try
            {
                _service.RestoreRecord(id);
                Console.WriteLine($"Record #{id} was restored");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

    }
}
