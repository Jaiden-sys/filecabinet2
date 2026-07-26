using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace filecabinet
{
    internal class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly DateTime limitedDateOfBirth = new DateTime(1950, 1, 1);
        private readonly CultureInfo culture = CultureInfo.InvariantCulture;
        /// <summary>
        /// This method allows to create records
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="dateOfBirth"></param>
        /// <param name="archiveId"></param>
        /// <param name="weight"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth, short archiveId, decimal weight, char type)
        {
            if (list == null) { throw new ArgumentException("list is empty"); }
            if (firstName == null) { throw new ArgumentNullException(nameof(firstName), "firstname cannot be null"); }
            if (lastName == null) { throw new ArgumentNullException(nameof(lastName), "lastname cannot be null"); }
            if (firstName.Length < 2 || firstName.Length > 60 || string.IsNullOrWhiteSpace(firstName)) throw new ArgumentException("Invalid first name", nameof(firstName));

            if (lastName.Length < 2 || lastName.Length > 60 || string.IsNullOrWhiteSpace(lastName)) throw new ArgumentException("Invalid last name", nameof(lastName));

            if (dateOfBirth == default(DateTime))
                throw new ArgumentException("Date of birth is not specified", nameof(dateOfBirth));

            if (dateOfBirth < limitedDateOfBirth || dateOfBirth > DateTime.Today)
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
            if (!firstNameDictionary.TryGetValue(firstName, out List<FileCabinetRecord>? nameList))
            {
                nameList = new List<FileCabinetRecord>();
                firstNameDictionary.Add(firstName, nameList);
            }
            nameList.Add(record);

            return record.Id;
        }
        /// <summary>
        /// Returns array with records or empty array
        /// </summary>
        public FileCabinetRecord[] GetRecords()
        {
            if (list.Count == 0) return Array.Empty<FileCabinetRecord>();
            else return this.list.ToArray();
        }
        /// <summary>
        /// Shows quantity of records
        /// </summary>
        /// <returns></returns>
        public int GetStat()
        {
            if ( list.Count == 0) return 0;
            else return this.list.Count;
        }
        
        /// <summary>
        /// Updates dictionary using dictionary with grouping
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="oldKey"></param>
        /// <param name="newKey"></param>
        /// <param name="record"></param>
        private static void UpdateDictionary(
            Dictionary<string, List<FileCabinetRecord>> dictionary,
            string oldKey,
            string newKey,
            FileCabinetRecord record)
        {
            if (oldKey == newKey) return;
            
            if(dictionary.TryGetValue(oldKey, out var oldList))
            {
                oldList.Remove(record);
                if(oldList.Count == 0) dictionary.Remove(oldKey);
            }

            if(!dictionary.TryGetValue(newKey, out var newList))
            {
                newList = new List<FileCabinetRecord>();
                dictionary.Add(newKey, newList);
            }
            newList.Add(record);
        }
        private FileCabinetRecord? FindById(int id) => list.FirstOrDefault(x => x.Id == id);
        public void EditRecord(
            int id, 
            string firstName, 
            string lastName, 
            DateTime dateOfBirth, 
            short archiveId, 
            decimal weight, 
            char type)
        {
            FileCabinetRecord? recordToUpdate = FindById(id);   
            
            if (recordToUpdate == null) throw new ArgumentException("Not found (id)");
            string? oldFirstName = recordToUpdate.FirstName;
            string? oldLastName = recordToUpdate.LastName;
            string? oldBirthDate = recordToUpdate.DateOfBirth.ToString("yyyyMMdd",culture);

            recordToUpdate.FirstName = firstName;
            recordToUpdate.LastName = lastName;
            recordToUpdate.DateOfBirth = dateOfBirth;
            recordToUpdate.ArchiveId = archiveId;
            recordToUpdate.Weight = weight;
            recordToUpdate.Type = type;
            if (oldFirstName == null || oldLastName == null) return;
            UpdateDictionary(firstNameDictionary, oldFirstName, firstName, recordToUpdate);
            UpdateDictionary(lastNameDictionary,oldLastName, lastName, recordToUpdate);
            UpdateDictionary(dateOfBirthDictionary, oldBirthDate, dateOfBirth.ToString("yyyyMMdd", culture), recordToUpdate);
            

            Console.WriteLine($"#{recordToUpdate.Id} was updated");

        }
            
        

        public FileCabinetRecord[] FindByField(string fieldName, string value)
        {
            switch (fieldName.ToLower(culture))
            {
                
                case "firstname":
                    return firstNameDictionary[value].ToArray();
                case "lastname":
                    return lastNameDictionary[value].ToArray();
                case "dateofbirth":
                    return dateOfBirthDictionary[value].ToArray();
                default:
                    throw new ArgumentException($"Field {fieldName} isn't supported");
            }

        }
    }
}
