using System;
using System.Collections.Generic;
using System.Text;
using filecabinet;
namespace filecabinet
{
    public class UndoCaretaker
    {
        private readonly Stack<IMemento> history = new Stack<IMemento>();
        public bool CanUndo => history.Count > 0;
        public void Save(IMemento memento) => history.Push(memento);
        public IMemento Undo()
        {
            if(!CanUndo)
                throw new InvalidOperationException("Nothing to undo.");
            return history.Pop();
        }
    }
}
