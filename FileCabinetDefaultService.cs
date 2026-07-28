using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace filecabinet
{
    public class FileCabinetDefaultService : FileCabinetService
    {
        protected override IRecordValidator CreateValidator()
        {
            return new DefaultValidator(); 
        }
    }
}
