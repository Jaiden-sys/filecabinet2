using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace filecabinet
{
    public class FileCabinetDefaultService : FileCabinetService
    {
        public FileCabinetDefaultService() : base(new DefaultValidator()){}
    }
}
