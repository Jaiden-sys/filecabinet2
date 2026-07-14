using System;
using System.Collections.Generic;
using System.Text;

namespace filecabinet
{
    public class FileCabinetRecord
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }
        public short ArchiveId { get; set; }
        public decimal Weight { get; set; }
        public char Type {  get; set; }
    }
}
