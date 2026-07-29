using System.ComponentModel;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

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

        static IRecordValidator validator = new DefaultValidator();
        private static IFileCabinetService fileCabinetService = new FileCabinetService(validator);
        
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
        private static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            do
            {
                T value;

                var input = Console.ReadLine();
                var conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"Validation failed: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

                return value;
            }
            while (true);
        }


        private static Func<string, Tuple<bool, string, string>> stringConverter = input =>
        {
            if (!string.IsNullOrWhiteSpace(input)) return Tuple.Create(true, string.Empty, input.Trim());
            return Tuple.Create(false, "Invalid string format", string.Empty );
        };
        private static Func<string, Tuple<bool, string>> nameValidator = name =>
        {
            if (name.Length >= 2 && name.Length <= 50)
                return Tuple.Create(true, string.Empty);
            return Tuple.Create(false, "Name lenght must be between 2 and 50");
        };


        private static Func<string, Tuple<bool, string, decimal>> weightConverter = input =>
        {
            if (decimal.TryParse(input, out var result))
                return Tuple.Create(true, string.Empty, result);
            return Tuple.Create(false, "Invalid decimal format", 0m);
        };
        private static Func<decimal, Tuple<bool, string>> weightValidator = weight => 
        {
            if (weight >= 0 && weight <= 200)
                return Tuple.Create(true, string.Empty);
            return Tuple.Create(false, "Weight must be between 0 and 200");
        };


        private static Func<string, Tuple<bool, string, DateTime>> dateTimeConverter = input =>
        {
            if (DateTime.TryParse(input, out DateTime result))
            {
                return Tuple.Create(true, string.Empty, result);
            }

            return Tuple.Create(false, "Invalid date format", default(DateTime));
        };
        private static Func<DateTime, Tuple<bool, string>> dateTimeValidator = dateTime =>
        {
            if(dateTime > new DateTime(1950, 1, 1) && dateTime < DateTime.Today)
            {
                return Tuple.Create(true, dateTime.ToString(CultureInfo.InvariantCulture));
            }
            return Tuple.Create(false, "Date must been between 1950 and today");
        };


        private static Func<string, Tuple<bool, string, short>> archiveIdConverter = input =>
        {
            if(short.TryParse(input, out short result))
            {
                return Tuple.Create(true, string.Empty, result);
            }
            return Tuple.Create(false,"Invalid archiveid",default(short));
        };
        private static Func<short, Tuple<bool, string>> archiveIdValidator = archiveId =>
        {
            if (archiveId >= 0)
            {
                return Tuple.Create(true, string.Empty);
            }
            return Tuple.Create(false, "invalid archiveid");
        };


        private static Func<string, Tuple<bool, string, char>> charConverter = input =>
        {
            if (char.TryParse(input, out char result))
            {
                return Tuple.Create(true, string.Empty, result);
            }
            return Tuple.Create(false, "Invalid type", default(char));
        };
        private static Func<char, Tuple<bool, string>> charValidator = type =>
        {
            if (type != ' ')
            {
                return Tuple.Create(true, string.Empty);
            }
            return Tuple.Create(false, "invalid type");
        };

        static void Create(string parameters)
        {
            
            Console.Write("First name: ");
            string firstName = ReadInput(stringConverter, nameValidator);

            Console.Write("Second name: ");
            string lastname = ReadInput(stringConverter, nameValidator);

            Console.Write("Date of birth, format (YYYY-MM-DD): ");
            DateTime dateOfBirth = ReadInput(dateTimeConverter, dateTimeValidator);

            Console.Write("ArchiveId: ");
            short archiveId = ReadInput(archiveIdConverter, archiveIdValidator);

            Console.Write("Weight: ");
            decimal weight = ReadInput(weightConverter, weightValidator);


            Console.Write("Type(char): ");
            char type = ReadInput(charConverter, charValidator);
            
            var request = new RecordRequest(0,firstName, lastname, dateOfBirth,archiveId,weight,type);

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
            if (int.TryParse(parameters, out id))
            { }
            else
            {
                Console.WriteLine("Invalid ID format.");
                return;
            }
            Console.Write("New first name: ");
            string firstName = ReadInput(stringConverter, nameValidator);

            Console.Write("New second name: ");
            string lastname = ReadInput(stringConverter, nameValidator);

            Console.Write("New date of birth, format (YYYY-MM-DD): ");
            DateTime dateOfBirth = ReadInput(dateTimeConverter, dateTimeValidator);

            Console.Write("New archiveId: ");
            short archiveId = ReadInput(archiveIdConverter, archiveIdValidator);

            Console.Write("New weight: ");
            decimal weight = ReadInput(weightConverter, weightValidator);


            Console.Write("New type(char): ");
            char type = ReadInput(charConverter, charValidator);

            var request = new RecordRequest(id,firstName,lastname,dateOfBirth,archiveId,weight,type);
                fileCabinetService.EditRecord(request);
            

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
