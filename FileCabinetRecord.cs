using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace filecabinet
{
    public class FileCabinetRecord
    {
        public int Id { get; set; }

        private string _firstName;
        public string FirstName { get => _firstName; set => _firstName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value?.ToLower() ?? string.Empty); }

        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }
        public short ArchiveId { get; set; }
        public decimal Weight { get; set; }
        public char Type {  get; set; }
    }
}
