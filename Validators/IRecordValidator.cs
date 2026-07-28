using System;
using System.Collections.Generic;
using System.Text;
namespace filecabinet
{
    public interface IRecordValidator
    {
        void ValidateParameters(RecordRequest request);
    }
}
