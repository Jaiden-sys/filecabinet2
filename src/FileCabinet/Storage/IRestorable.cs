using System;
using System.Collections.Generic;
using System.Text;

namespace filecabinet.Storage
{
    public interface IRestorable
    {
        public void RestoreRecord(int id);
    }
}
