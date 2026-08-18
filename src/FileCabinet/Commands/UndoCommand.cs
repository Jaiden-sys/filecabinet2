using System;
using System.Collections.Generic;
using System.Text;

namespace filecabinet.Commands
{
    public class UndoCommand : ICommand
    {
        public string Name => "undo";
        public string HelpText => "undoes last deletion";
        private readonly IFileCabinetService _service;
        private readonly IUndoOriginator _originator;
        private readonly UndoCaretaker _caretaker;
        public UndoCommand(IFileCabinetService service, IUndoOriginator originator, UndoCaretaker caretaker)
        {
            _service = service;
            _originator = originator;
            _caretaker = caretaker;
        }
        public void Execute(string parameters) 
        {
            if (_originator is null || !_caretaker.CanUndo)
            {
                Console.WriteLine("Nothing to undo.");
                return;
            }

            _originator.Restore(_caretaker.Undo());
            Console.WriteLine("Last operation was undo");
        }

    }
}
