using filecabinet;
using filecabinet.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Windows.Input;
using filecabinet.Commands;
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
        
    
        private static IRecordValidator validator = null!;
        private static IUndoOriginator? originator;
        private static IRestorable? restorable;
        private static UndoCaretaker caretaker = new();
        private static Tuple<string, Action<string>>[] commands = null!;

        private static IFileCabinetService fileCabinetService = null!;



        internal static List<string[]> helpMessages = new List<string[]>
        {
            new[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new[] { "stat", "shows stat of records", "The 'stat' command prints the statistics of records." },
            new[] { "create", "creates new record", "The 'create' command creates new record in app" },
            new[] { "list", "shows list of all records created in app", "The 'list' command shows the list of all records" },
            new[] { "edit", "edits chosen record", "The 'edit' command edits records" },
            new[] { "find", "finds record", "The 'find' command allows you to find record" },
            new[] {"remove", "removes record", "The 'remove command removes record"}
        };
        

        public static void Main(string[] args)
        {
            string storageMode = "efcore";
            string validationMode = "default";
            foreach (var arg in args)
            {
                if (arg.StartsWith("--storage="))
                {
                    storageMode = arg["--storage=".Length..]; //slices
                }
                else if (arg.StartsWith("--validation-rules="))
                {
                    validationMode = arg["--validation-rules=".Length..];
                }
            }
            validator = validationMode switch
            {
                "custom" => new CustomValidator(),
                _ => new DefaultValidator(),
            };
            switch(storageMode)
            {
                case "efcore":
                        var options = new DbContextOptionsBuilder<FileCabinetDbContext>()
                            .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=FileCabinet;Trusted_Connection=True;TrustServerCertificate=True;")
                            .Options;
                    var dbContext = new FileCabinetDbContext(options);
                    var efService = new EfFileCabinetService(validator, dbContext);
                    fileCabinetService = efService;
                    restorable = efService;
                    break;
                default:
                    var memoryService = new FileCabinetService(validator);

                    fileCabinetService = memoryService;
                    originator = memoryService;
                    break;
            }
            
            var commandPatternList = new List<Commands.ICommand>();
            var helpCommand = new HelpCommand(commandPatternList);
            commandPatternList.Add(helpCommand);
            commandPatternList.Add(new CreateCommand(fileCabinetService));
            commandPatternList.Add(new EditCommand(fileCabinetService));
            commandPatternList.Add(new StatCommand(fileCabinetService));
            commandPatternList.Add(new FindCommand(fileCabinetService));
            commandPatternList.Add(new ListCommand(fileCabinetService));
            commandPatternList.Add(new RemoveCommand(fileCabinetService,originator,caretaker));
            commandPatternList.Add(new ExitCommand());

            if (originator is not null)
            {
                
                commandPatternList.Add(new UndoCommand(fileCabinetService, originator, caretaker));
                helpMessages.Add(new[] { "undo", "Undoes last operation", "Use 'undo' command to revert last changes" });
            }
            if(restorable is not null)
            {
                
                commandPatternList.Add(new RestoreCommand(restorable, fileCabinetService));
                helpMessages.Add(new[] { "restore", "Restores record deletion", "Use 'restore' command to cancel deletion" });
            }
            
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

                    var index = Array.FindIndex(commandPatternList.ToArray(), 0, commandPatternList.Count,
                        i => i.Name.Equals(command, StringComparison.InvariantCultureIgnoreCase));

                    if (index >= 0)
                    {
                        var parameters = inputs.Length > 1 ? inputs[1] : string.Empty;
                        commandPatternList[index].Execute(parameters);
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


    }
}