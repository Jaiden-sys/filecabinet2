using System;
using System.Collections.Generic;
using System.Text;

namespace filecabinet
{
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        public int CreateRecord(string firstName, string lastName, DateTime? dateOfBirth, short archiveId, decimal weight, char type)
        {
            if(firstName == null) { throw new ArgumentNullException(nameof(firstName), "firstname cannot be null"); }
            if (lastName == null) { throw new ArgumentNullException(nameof(lastName), "lastname cannot be null"); }
            if (firstName.Length < 2 || firstName.Length > 60 || string.IsNullOrWhiteSpace(firstName))  throw new ArgumentException("Invalid first name", nameof(firstName));

            if (lastName.Length < 2 || lastName.Length > 60 || string.IsNullOrWhiteSpace(lastName)) throw new ArgumentException("Invalid last name", nameof(lastName));

            if (dateOfBirth == null)
                throw new ArgumentNullException(nameof(dateOfBirth), "Date of birth is required");

            if (!dateOfBirth.HasValue || dateOfBirth < new DateTime(1950,1,1) || dateOfBirth > DateTime.Today)
                throw new ArgumentException("Invalid date of birth", nameof(dateOfBirth));

            if (archiveId <= 0)
                throw new ArgumentOutOfRangeException(nameof(archiveId), "Archive ID must be positive");

            if (weight <= 0 || weight > 200)
                throw new ArgumentOutOfRangeException(nameof(weight), "Weight must be between 0 and 200");

            if (type == ' ') 
                throw new ArgumentException("Type cannot be empty/space", nameof(type));
            var record = new FileCabinetRecord
                {
                    Id = this.list.Count + 1,
                    FirstName = firstName,
                    LastName = lastName,
                    DateOfBirth = dateOfBirth,
                    ArchiveId = archiveId,
                    Weight = weight,
                    Type = type
                };
                this.list.Add(record);
                return record.Id;
        }

        public FileCabinetRecord[] GetRecords()
        {
            
            return this.list.ToArray();
        }

        public int GetStat()
        {
            return this.list.Count;
        }
    }
}
