using System.Globalization;
using System.Reflection.Metadata.Ecma335;

namespace filecabinet
{
    public static class Program
    {
        private const string DeveloperName = "Roman Eliseev";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private static bool isRunning = true;

        private static FileCabinetService fileCabinetService = new FileCabinetCustomService();
        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple <string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create",Create),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("find", Find)
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "shows stat of records", "The 'stat' command prints the statistics of records." },
            new string[] { "create", "creates new record", "The 'create' command creates new record in app"},
            new string[] { "list", "shows list of all records created in app","The 'list' command shows the list of all records"},
            new string[] {"edit", "edits chosen record", "The 'edit' command edits records"},
            new string[] {"find", "finds record", "The 'find' command allow you to find record"}
        };

        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();

            do
            {
                Console.Write("> ");
                var line = Console.ReadLine();
                var inputs = line != null ? line.Split(' ', 2) : new string[] { string.Empty, string.Empty };
                const int commandIndex = 0;
                var command = inputs[commandIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Program.HintMessage);
                    continue;
                }

                var index = Array.FindIndex(commands, 0, commands.Length, i => i.Item1.Equals(command, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    const int parametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                    commands[index].Item2(parameters);
                }
                else
                {
                    PrintMissedCommandInfo(command);
                }
            }
            while (isRunning);
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[Program.CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(helpMessages[index][Program.ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in helpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[Program.CommandHelpIndex], helpMessage[Program.DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            isRunning = false;
        }
        private static void Stat(string parameters)
        {
            var recordsCount = Program.fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }
        private static string GetInput()
        {
            string? input;
            do
            {
                input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Error: empty input");
                }
            } while (string.IsNullOrWhiteSpace(input));
            return input.Trim();
        }
       static void Create(string parameters)
        {
            //TODO: Rework dateOfBith, archiveId, weight, type checks
            //Implement generic check
            Console.Write("First name: ");
            string firstName = GetInput();

            Console.Write("Second name: ");
            string secondName = GetInput();

            DateTime dateOfBirth;
            while (true)
            {
                Console.Write("Date of birth, format (YYYY-MM-DD): ");
                string? input = Console.ReadLine();
                if (DateTime.TryParse(input, out dateOfBirth)) break;
                Console.WriteLine("Error: Invalid date format.");
            }


            short archiveId;
            while (true)
            {
                Console.Write("ArchiveId: ");
                string? input = Console.ReadLine();
                if (short.TryParse(input, out archiveId)) break;
                Console.WriteLine("Error: Invalid archiveId format.");
            }

            decimal weight;
            while (true)
            {
                Console.Write("Weight: ");
                string? input = Console.ReadLine();
                if (decimal.TryParse(input, out weight)) break;
                Console.WriteLine("Error: Invalid weight format.");
            }


            char type;
            while (true)
            {
                Console.Write("Type(char): ");
                string? input = Console.ReadLine();
                if (char.TryParse(input, out type)) break;
                Console.WriteLine("Error: Invalid char format.");
            }
            var request = new FileCabinetService.RecordRequest(0,firstName, secondName,dateOfBirth,archiveId,weight,type);

            int recordId = fileCabinetService.CreateRecord(request);

            Console.WriteLine($"Record #{recordId} has been created.");



        }
        private static void List(string parameters)
        {
            var records = Program.fileCabinetService.GetRecords();

            foreach (var record in records)
            {
                Console.WriteLine($"#{record.Id},{record.FirstName},{record.LastName},{record.DateOfBirth},{record.ArchiveId},{record.Weight},{record.Type}");
            }

        }

        private static void Edit(string parameters)
        {
            int id = 0;
            if (!string.IsNullOrWhiteSpace(parameters))
            {

                if (int.TryParse(parameters, out id))
                { }
                else
                {
                    Console.WriteLine("Invalid ID format.");
                }

                Console.Write("New first name: ");
                string firstName = GetInput();

                Console.Write("New second name: ");
                string lastname = GetInput();

                DateTime dateOfBirth;
                while (true)
                {
                    Console.Write("New date of birth, format (YYYY-MM-DD): ");
                    string? input = Console.ReadLine();
                    if (DateTime.TryParse(input, out dateOfBirth)) break;
                    Console.WriteLine("Error: Invalid date format.");
                }


                short archiveId;
                while (true)
                {
                    Console.Write("New archiveId: ");
                    string? input = Console.ReadLine();
                    if (short.TryParse(input, out archiveId)) break;
                    Console.WriteLine("Error: Invalid archiveId format.");
                }

                decimal weight;
                while (true)
                {
                    Console.Write("New weight: ");
                    string? input = Console.ReadLine();
                    if (decimal.TryParse(input, out weight)) break;
                    Console.WriteLine("Error: Invalid weight format.");
                }


                char type;
                while (true)
                {
                    Console.Write("New type(char): ");
                    string? input = Console.ReadLine();
                    if (char.TryParse(input, out type)) break;
                    Console.WriteLine("Error: Invalid char format.");
                }
                var request = new FileCabinetService.RecordRequest(id,firstName,lastname,dateOfBirth,archiveId,weight,type);
                fileCabinetService.EditRecord(request);
            }

        }
        private static void Find(string parameters)
        {
            
            if (string.IsNullOrWhiteSpace(parameters))
            {
                Console.WriteLine("Usage: find <firstname|lastname> <value>");
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

            if (field == "firstname")
            {
                results = fileCabinetService.FindByField("firstname", value);
            }
            else if (field == "lastname")
            {
                results = fileCabinetService.FindByField("lastname", value);
            }
            else if (field == "dateofbirth")
            {
                results = fileCabinetService.FindByField("dateofbirth", value);
            }
            else
            {
                Console.WriteLine($"Error: search by '{field}' is not supported. Use 'firstname' or 'lastname'.");
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
