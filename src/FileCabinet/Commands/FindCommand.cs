using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace filecabinet.Commands
{
    public class FindCommand : ICommand
    {
        public string Name => "find";
        public string HelpText => "Finds record";
        private readonly IFileCabinetService _service;
        public FindCommand(IFileCabinetService service)
        {  this._service = service;}
        public void Execute(string parameters) 
        {
            if (string.IsNullOrWhiteSpace(parameters))
            {
                Console.WriteLine("Usage: find <firstname|lastname|dateofbirth> <value>");
                return;
            }

            string[] parts = parameters.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 2)
            {
                Console.WriteLine("Error: please specify both the field and the value.");
                return;
            }

            string field = parts[0].ToLower(CultureInfo.InvariantCulture);
            string value = string.Join(" ", parts.Skip(1));

            IEnumerable<FileCabinetRecord> results;

            if (field == "firstname" || field == "lastname" || field == "dateofbirth")
            {
                results = _service.FindByField(field, value);
            }
            else
            {
                Console.WriteLine($"Error: search by '{field}' is not supported. Use 'firstname', 'lastname' or 'dateofbirth'.");
                return;
            }

            if (results != null && results.Any())
            {
                foreach (var record in results)
                {
                    Console.WriteLine($"{record.Id}, {record.FirstName}, {record.LastName}, {record.DateOfBirth:d}, {record.ArchiveId}, {record.Weight}, {record.Type}");
                }
            }
            else
            {
                Console.WriteLine("No records found.");
            }
        }
    }
}
