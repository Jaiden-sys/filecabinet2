using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace filecabinet.Commands
{
    public class RemoveCommand : ICommand
    {
        
       
        public string Name => "remove";
        public string HelpText => "Removes record";
        private readonly IFileCabinetService _service;
        private readonly IUndoOriginator? _originator;
        private readonly UndoCaretaker _caretaker;
        public RemoveCommand(IFileCabinetService service, IUndoOriginator originator, UndoCaretaker caretaker)
        {
            _service = service;
            _originator = originator;
            _caretaker = caretaker;
        }
        public void Execute(string parameters) 
        {
            if (!int.TryParse(parameters, out int id))
            {
                Console.WriteLine("Invalid id.");
                return;
            }
            try
            {
                if (_originator is not null)
                {
                    _caretaker.Save(_originator.CreateMemento());
                }
                _service.RemoveRecord(id);
                Console.WriteLine($"Record #{id} was removed");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
