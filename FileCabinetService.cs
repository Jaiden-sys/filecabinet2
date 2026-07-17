using System;
using System.Collections.Generic;
using System.Text;

namespace filecabinet
{
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth, short archiveId, decimal weight, char type)
        {
            if(firstName == null) { throw new ArgumentNullException(nameof(firstName), "firstname cannot be null"); }
            if (lastName == null) { throw new ArgumentNullException(nameof(lastName), "lastname cannot be null"); }
            if (firstName.Length < 2 || firstName.Length > 60 || string.IsNullOrWhiteSpace(firstName))  throw new ArgumentException("Invalid first name", nameof(firstName));

            if (lastName.Length < 2 || lastName.Length > 60 || string.IsNullOrWhiteSpace(lastName)) throw new ArgumentException("Invalid last name", nameof(lastName));

            if (dateOfBirth == default(DateTime))
                throw new ArgumentException("Date of birth is not specified", nameof(dateOfBirth));

            if (dateOfBirth < new DateTime(1950,1,1) || dateOfBirth > DateTime.Today)
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

        public void EditRecord(int id, string firstName, string lastName, DateTime dateOfBirth, short archiveId, decimal weight, char type)
        {
            var record = this.list.Find(x => x.Id == id);
            if (record == null) throw new ArgumentException("Cannot find record with current id ");
            record.FirstName = firstName;
            record.LastName = lastName;
            record.DateOfBirth = dateOfBirth;
            record.ArchiveId = archiveId;
            record.Weight = weight;
            record.Type = type;
            Console.WriteLine($"#{record.Id} was updated");
        }
    }
}
