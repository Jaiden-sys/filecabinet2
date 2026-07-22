using System;
using System.Collections.Generic;
using System.Text;

namespace filecabinet
{
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth, short archiveId, decimal weight, char type)
        {
            if (firstName == null) { throw new ArgumentNullException(nameof(firstName), "firstname cannot be null"); }
            if (lastName == null) { throw new ArgumentNullException(nameof(lastName), "lastname cannot be null"); }
            if (firstName.Length < 2 || firstName.Length > 60 || string.IsNullOrWhiteSpace(firstName)) throw new ArgumentException("Invalid first name", nameof(firstName));

            if (lastName.Length < 2 || lastName.Length > 60 || string.IsNullOrWhiteSpace(lastName)) throw new ArgumentException("Invalid last name", nameof(lastName));

            if (dateOfBirth == default(DateTime))
                throw new ArgumentException("Date of birth is not specified", nameof(dateOfBirth));

            if (dateOfBirth < new DateTime(1950, 1, 1) || dateOfBirth > DateTime.Today)
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

            list.Add(record);
            //nameList is the List with records satisfying firstname
            if (!firstNameDictionary.TryGetValue(firstName, out List<FileCabinetRecord> nameList))
            {
                nameList = new List<FileCabinetRecord>();
                firstNameDictionary.Add(firstName, nameList);
            }
            nameList.Add(record);

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
            FileCabinetRecord? recordToUpdate = null;
            string? oldFirstName = null;
            foreach (var pair in firstNameDictionary)
            {
                var found = pair.Value.FirstOrDefault(r => r.Id == id);
                if (found != null)
                {
                    recordToUpdate = found;
                    oldFirstName = pair.Key;
                    break;
                }
            }
            if (recordToUpdate == null) throw new ArgumentException("Record not found");

            if(oldFirstName != firstName)
            {
                var oldList = firstNameDictionary[oldFirstName];
                oldList.Remove(recordToUpdate);

                if(oldList.Count == 0)
                {
                    firstNameDictionary.Remove(oldFirstName);
                }

                if(!firstNameDictionary.TryGetValue(firstName,out var newList))
                {
                    newList = new List<FileCabinetRecord>();
                    firstNameDictionary.Add(firstName, newList);
                }
                newList.Add(recordToUpdate);
            }
            recordToUpdate.FirstName = firstName;
            recordToUpdate.LastName = lastName;
            recordToUpdate.DateOfBirth = dateOfBirth;
            recordToUpdate.ArchiveId = archiveId;
            recordToUpdate.Weight = weight;
            recordToUpdate.Type = type;
            Console.WriteLine($"#{recordToUpdate.Id} was updated");

        }
            
        

        public FileCabinetRecord[] FindByField(string fieldName, string value)
        {
            switch (fieldName.ToLower())
            {
                case "firstname":
                    return list.Where(x => x.FirstName.ToLower() == value.ToLower()).ToArray();
                case "lastname":
                    return list.Where(x => x.LastName.ToLower() == value.ToLower()).ToArray();
                case "dateofbirth":
                    return list.Where(x => x.DateOfBirth.ToString(value) == value.ToLower()).ToArray();
                default:
                    throw new ArgumentException($"Field {fieldName} isn't supported");
            }

        }
    }
}
