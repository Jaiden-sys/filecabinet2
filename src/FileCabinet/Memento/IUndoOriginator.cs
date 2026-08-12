using System;
using System.Collections.Generic;
using System.Text;
using static filecabinet.FileCabinetService;

namespace filecabinet
{
    public interface IUndoOriginator
    {
        IMemento CreateMemento();
        void Restore(IMemento memento);
    }
}
