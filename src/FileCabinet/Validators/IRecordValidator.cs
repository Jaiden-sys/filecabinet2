using System;
using System.Collections.Generic;
using System.Text;
namespace filecabinet
{
    public interface IRecordValidator
    {
        public void ValidateParameters(RecordRequest request);
    }
}
