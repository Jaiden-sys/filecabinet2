using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Security.Cryptography;
using filecabinet;
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
        private static DbContextOptions<FileCabinetDbContext> options = new DbContextOptionsBuilder<FileCabinetDbContext>()
            .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=FileCabinet;Trusted_Connection=True;TrustServerCertificate=True;")
            .Options;
        private static FileCabinetDbContext? dbContext = new FileCabinetDbContext(options);
        private static readonly IRecordValidator validator = new DefaultValidator();
        private static readonly EfFileCabinetService serviceCore =
            new EfFileCabinetService(validator, dbContext);

        private static readonly IFileCabinetService fileCabinetService =
            serviceCore;

        //private static readonly IUndoOriginator originator =
          //  serviceCore;


        private static readonly Tuple<string, Action<string>>[] commands =
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("find", Find)
            //new Tuple<string, Action<string>>("remove", Remove),
            //new Tuple<string, Action<string>>("undo",Undo)
        };

        private static readonly string[][] helpMessages =
        {
            new[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new[] { "stat", "shows stat of records", "The 'stat' command prints the statistics of records." },
            new[] { "create", "creates new record", "The 'create' command creates new record in app" },
            new[] { "list", "shows list of all records created in app", "The 'list' command shows the list of all records" },
            new[] { "edit", "edits chosen record", "The 'edit' command edits records" },
            new[] { "find", "finds record", "The 'find' command allows you to find record" },
            new[] {"remove", "removes chosen record from list", "The 'remove' command allows you to delete record" },
            new[] {"undo",  "undo last operation","Use this command to undo last operation"}
        };

        public static void Main(string[] args)
        {
            
            Console.WriteLine($"File Cabinet Application, developed by {DeveloperName}");
            Console.WriteLine(HintMessage);
            Console.WriteLine();

            do
            {
                Console.Write("> ");
                var line = Console.ReadLine();
                var inputs = line != null ? line.Split(' ', 2) : new[] { string.Empty, string.Empty };
                var command = inputs[0];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(HintMessage);
                    continue;
                }

                var index = Array.FindIndex(commands, 0, commands.Length,
                    i => i.Item1.Equals(command, StringComparison.InvariantCultureIgnoreCase));

                if (index >= 0)
                {
                    var parameters = inputs.Length > 1 ? inputs[1] : string.Empty;
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
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length,
                    i => string.Equals(i[CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));

                if (index >= 0)
                {
                    Console.WriteLine(helpMessages[index][ExplanationHelpIndex]);
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
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[CommandHelpIndex], helpMessage[DescriptionHelpIndex]);
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
            var recordsCount = fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }

        private static void Create(string parameters)
        {
            Console.Write("First name: ");
            string firstName = ConsoleReader.ReadInput(ConsoleReader.StringConverter);

            Console.Write("Last name: ");
            string lastName = ConsoleReader.ReadInput(ConsoleReader.StringConverter);

            Console.Write("Date of birth, format (YYYY-MM-DD): ");
            DateTime dateOfBirth = ConsoleReader.ReadInput(ConsoleReader.DateTimeConverter);

            Console.Write("ArchiveId: ");
            short archiveId = ConsoleReader.ReadInput(ConsoleReader.ShortConverter);

            Console.Write("Weight: ");
            decimal weight = ConsoleReader.ReadInput(ConsoleReader.DecimalConverter);

            Console.Write("Type(char): ");
            char type = ConsoleReader.ReadInput(ConsoleReader.CharConverter);

            var request = new RecordRequest(0, firstName, lastName, dateOfBirth, archiveId, weight, type, isDeleted: false);

            try
            {
                int recordId = fileCabinetService.CreateRecord(request);
                Console.WriteLine($"Record #{recordId} has been created.");
            }
            catch (ArgumentException ex)
            {

                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine("Please try again.");
            }
        }

        private static void List(string parameters)
        {
            var records = fileCabinetService.GetRecords();

            foreach (var record in records)
            {
                Console.WriteLine($"#{record.Id},{record.FirstName},{record.LastName},{record.DateOfBirth},{record.ArchiveId},{record.Weight},{record.Type}");
            }
        }

        private static void Edit(string parameters)
        {
            if (!int.TryParse(parameters, out int id))
            {
                Console.WriteLine("Invalid ID format.");
                return;
            }

            Console.Write("New first name: ");
            string firstName = ConsoleReader.ReadInput(ConsoleReader.StringConverter);

            Console.Write("New last name: ");
            string lastName = ConsoleReader.ReadInput(ConsoleReader.StringConverter);

            Console.Write("New date of birth, format (YYYY-MM-DD): ");
            DateTime dateOfBirth = ConsoleReader.ReadInput(ConsoleReader.DateTimeConverter);

            Console.Write("New archiveId: ");
            short archiveId = ConsoleReader.ReadInput(ConsoleReader.ShortConverter);

            Console.Write("New weight: ");
            decimal weight = ConsoleReader.ReadInput(ConsoleReader.DecimalConverter);

            Console.Write("New type(char): ");
            char type = ConsoleReader.ReadInput(ConsoleReader.CharConverter);

            var request = new RecordRequest(id, firstName, lastName, dateOfBirth, archiveId, weight, type, isDeleted: false);

            try
            {
                fileCabinetService.EditRecord(request);
                Console.WriteLine($"Record #{id} has been updated.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void Find(string parameters)
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
                results = fileCabinetService.FindByField(field, value);
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
        private static readonly UndoCaretaker caretaker = new UndoCaretaker();
        /*
        public static void Remove(string parameters)
        {
            if (!int.TryParse(parameters, out int id))
            {
                Console.WriteLine("Invalid id.");
                return;
            }

            try
            {
                var memento = originator.CreateMemento();
                fileCabinetService.RemoveRecord(id);
                caretaker.Save(memento);
                Console.WriteLine($"Record #{id} was removed");
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }
        private static void Undo(string parameters)
        {
            if (!caretaker.CanUndo)
            {
                Console.WriteLine("Nothing to undo");
                return;
            }
            originator.Restore(caretaker.Undo());
            Console.WriteLine("Last operation was undo");
        }
        */
    }
}