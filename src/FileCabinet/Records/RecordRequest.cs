using System;
using System.Collections.Generic;
using System.Text;

namespace filecabinet
{
    public record RecordRequest(
            int Id,
            string FirstName,
            string LastName,
            DateTime DateOfBirth,
            short ArchiveId,
            decimal Weight,
            char Type);
}
