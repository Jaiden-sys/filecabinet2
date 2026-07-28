using filecabinet;
using System;
using System.Collections.Generic;
using System.Text;

namespace filecabinet
{
    public class FileCabinetCustomService : FileCabinetService
    {
        public FileCabinetCustomService() : base(new CustomValidator()) { }
    }
}

