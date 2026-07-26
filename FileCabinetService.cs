using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
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
        public record RecordRequest(
            int Id,
            string FirstName,
            string LastName,
            DateTime DateOfBirth,
            short ArchiveId,
            decimal Weight,
            char Type);
        
        
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
        public int CreateRecord(RecordRequest request)
        {
            if (list == null) { throw new ArgumentException("list is empty"); }
            if (request.FirstName == null) { throw new ArgumentNullException(nameof(request.FirstName), "firstname cannot be null"); }
            if (request.LastName == null) { throw new ArgumentNullException(nameof(request.LastName), "lastname cannot be null"); }
            if (request.FirstName.Length < 2 || request.FirstName.Length > 60 || string.IsNullOrWhiteSpace(request.FirstName)) throw new ArgumentException("Invalid first name", nameof(request.FirstName));

            if (request.LastName.Length < 2 || request.LastName.Length > 60 || string.IsNullOrWhiteSpace(request.LastName)) throw new ArgumentException("Invalid last name", nameof(request.LastName));

            if (request.DateOfBirth == default(DateTime))
                throw new ArgumentException("Date of birth is not specified", nameof(request.DateOfBirth));

            if (request.DateOfBirth < limitedDateOfBirth || request.DateOfBirth > DateTime.Today)
                throw new ArgumentException("Invalid date of birth", nameof(request.DateOfBirth));

            if (request.ArchiveId <= 0)
                throw new ArgumentOutOfRangeException(nameof(request.ArchiveId), "Archive ID must be positive");

            if (request.Weight <= 0 || request.Weight > 200)
                throw new ArgumentOutOfRangeException(nameof(request.Weight), "Weight must be between 0 and 200");

            if (request.Type == ' ')
                throw new ArgumentException("Type cannot be empty/space", nameof(request.Type));
            var record = new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
                ArchiveId = request.ArchiveId,
                Weight = request.Weight,
                Type = request.Type
            };

            list.Add(record);
            //nameList is the List with records satisfying firstname
            if (!firstNameDictionary.TryGetValue(request.FirstName, out List<FileCabinetRecord>? nameList))
            {
                nameList = new List<FileCabinetRecord>();
                firstNameDictionary.Add(request.FirstName, nameList);
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
        public void EditRecord(RecordRequest request)
        {
            FileCabinetRecord? recordToUpdate = FindById(request.Id);   
            
            if (recordToUpdate == null) throw new ArgumentException("Not found (id)");
            string? oldFirstName = recordToUpdate.FirstName;
            string? oldLastName = recordToUpdate.LastName;
            string? oldBirthDate = recordToUpdate.DateOfBirth.ToString("yyyyMMdd",culture);

            recordToUpdate.FirstName = request.FirstName;
            recordToUpdate.LastName = request.LastName;
            recordToUpdate.DateOfBirth = request.DateOfBirth;
            recordToUpdate.ArchiveId = request.ArchiveId;
            recordToUpdate.Weight = request.Weight;
            recordToUpdate.Type = request.Type;
            if (oldFirstName == null || oldLastName == null) return;
            UpdateDictionary(firstNameDictionary, oldFirstName,request.FirstName , recordToUpdate);
            UpdateDictionary(lastNameDictionary,oldLastName, request.LastName , recordToUpdate);
            UpdateDictionary(dateOfBirthDictionary, oldBirthDate, request.DateOfBirth.ToString("yyyyMMdd", culture), recordToUpdate);
            

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
