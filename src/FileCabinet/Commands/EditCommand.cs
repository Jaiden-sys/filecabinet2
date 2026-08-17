using System;
using System.Collections.Generic;
using System.Text;

namespace filecabinet.Commands
{
    public class EditCommand : ICommand
    {
        public string Name => "edit";
        public string HelpText => "Edits record";
        private readonly IFileCabinetService _service;
        public EditCommand(IFileCabinetService service)
        {
            this._service = service;
        }
        public void Execute(string parameters)
        {
            if (!int.TryParse(parameters, out int id))
            {
                Console.WriteLine("Invalid ID format.");
                return;
            }

            Console.Write("New first name: ");
            string firstName = ConsoleReader.ReadInput(ConsoleReader.stringConverter);

            Console.Write("New last name: ");
            string lastName = ConsoleReader.ReadInput(ConsoleReader.stringConverter);

            Console.Write("New date of birth, format (YYYY-MM-DD): ");
            DateTime dateOfBirth = ConsoleReader.ReadInput(ConsoleReader.DateTimeConverter);

            Console.Write("New archiveId: ");
            short archiveId = ConsoleReader.ReadInput(ConsoleReader.ShortConverter);

            Console.Write("New weight: ");
            decimal weight = ConsoleReader.ReadInput(ConsoleReader.DecimalConverter);

            Console.Write("New type(char): ");
            char type = ConsoleReader.ReadInput(ConsoleReader.CharConverter);
            var request = new RecordRequest(id, firstName, lastName, dateOfBirth, archiveId, weight, type);

            try
            {
                _service.EditRecord(request);
                Console.WriteLine($"Record #{id} has been updated.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

}
