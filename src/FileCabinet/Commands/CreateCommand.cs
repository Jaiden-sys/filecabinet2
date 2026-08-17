using filecabinet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
namespace filecabinet.Commands
{
    public class CreateCommand : ICommand
    {
        public string Name => "create";
        public string HelpText => "Creates new record";
        private readonly IFileCabinetService _service;
        CreateCommand(IFileCabinetService service)
        {
            this._service = service;
        }
        public void Execute(string parameters)
        {

            Console.Write("First name: ");
            var firstName = ConsoleReader.ReadInput(ConsoleReader.stringConverter);

            Console.Write("Last name: ");
            var lastName = ConsoleReader.ReadInput(ConsoleReader.stringConverter);

            Console.Write("Date of birth, format (YYYY-MM-DD): ");
            var dateOfBirth = ConsoleReader.ReadInput(ConsoleReader.DateTimeConverter);

            Console.Write("ArchiveId: ");
            var archiveId = ConsoleReader.ReadInput(ConsoleReader.ShortConverter);

            Console.Write("Weight: ");
            var weight = ConsoleReader.ReadInput(ConsoleReader.DecimalConverter);

            Console.Write("Type(char): ");
            var type = ConsoleReader.ReadInput(ConsoleReader.CharConverter);
            
            var request = new RecordRequest(
                Id: 0,
                FirstName: firstName,
                LastName: lastName,
                DateOfBirth: dateOfBirth,
                ArchiveId: archiveId,
                Weight: weight,
                Type: type);
            int id = _service.CreateRecord(request);
            Console.WriteLine($"Record #{id} was created.");
        }

    }
}
