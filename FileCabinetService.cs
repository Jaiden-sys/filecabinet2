using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace filecabinet
{
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> _list = new List<FileCabinetRecord>();
        protected readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        protected readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        protected readonly Dictionary<string, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        protected readonly DateTime limitedDateOfBirth = new DateTime(1950, 1, 1);
        protected readonly CultureInfo culture = CultureInfo.InvariantCulture;
        protected IRecordValidator validator;
        public FileCabinetService(IRecordValidator validator) { this.validator = validator; }
        
        

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
            this.validator.ValidateParameters(request);
            var record = new FileCabinetRecord
            {
                Id = this._list.Count + 1,
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
                ArchiveId = request.ArchiveId,
                Weight = request.Weight,
                Type = request.Type
            };

            _list.Add(record);
            //nameList is the List with records satisfying firstname
            if (!firstNameDictionary.TryGetValue(request.FirstName, out List<FileCabinetRecord>? nameList))
            {
                nameList = new List<FileCabinetRecord>();
                firstNameDictionary.Add(request.FirstName, nameList);
                lastNameDictionary.Add(request.LastName, nameList);
                dateOfBirthDictionary.Add(request.DateOfBirth.ToString(culture), nameList);
            }
            nameList.Add(record);

            return record.Id;
        }
        /// <summary>
        /// Returns array with records or empty array
        /// </summary>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            if (_list.Count == 0) return ReadOnlyCollection<FileCabinetRecord>.Empty;
            else return new ReadOnlyCollection<FileCabinetRecord>(_list);
        }
        /// <summary>
        /// Shows quantity of records
        /// </summary>
        /// <returns></returns>
        public int GetStat()
        {
            if (_list.Count == 0) return 0;
            else return new ReadOnlyCollection<FileCabinetRecord>(_list).Count;
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

            if (dictionary.TryGetValue(oldKey, out var oldList))
            {
                oldList.Remove(record);
                if (oldList.Count == 0) dictionary.Remove(oldKey);
            }

            if (!dictionary.TryGetValue(newKey, out var newList))
            {
                newList = new List<FileCabinetRecord>();
                dictionary.Add(newKey, newList);
            }
            newList.Add(record);
        }
        private FileCabinetRecord? FindById(int id) => _list.FirstOrDefault(x => x.Id == id);
        public void EditRecord(RecordRequest request)
        {
            if(request == null) throw new ArgumentNullException(nameof(request));
            FileCabinetRecord? recordToUpdate = FindById(request.Id);

            if (recordToUpdate == null) throw new ArgumentException("Not found (id)");
            string? oldFirstName = recordToUpdate.FirstName;
            string? oldLastName = recordToUpdate.LastName;
            string? oldBirthDate = recordToUpdate.DateOfBirth.ToString("yyyyMMdd", culture);

            recordToUpdate.FirstName = request.FirstName;
            recordToUpdate.LastName = request.LastName;
            recordToUpdate.DateOfBirth = request.DateOfBirth;
            recordToUpdate.ArchiveId = request.ArchiveId;
            recordToUpdate.Weight = request.Weight;
            recordToUpdate.Type = request.Type;
            if (oldFirstName == null || oldLastName == null) return;
            UpdateDictionary(firstNameDictionary, oldFirstName, request.FirstName, recordToUpdate);
            UpdateDictionary(lastNameDictionary, oldLastName, request.LastName, recordToUpdate);
            UpdateDictionary(dateOfBirthDictionary, oldBirthDate, request.DateOfBirth.ToString("yyyyMMdd", culture), recordToUpdate);


            Console.WriteLine($"#{recordToUpdate.Id} was updated");

        }



        public ReadOnlyCollection<FileCabinetRecord> FindByField(string fieldName, string value)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
            {
                throw new ArgumentNullException(nameof(fieldName), "Имя поля не может быть пустым");
            }

            if (value == null)
            {
                return ReadOnlyCollection<FileCabinetRecord>.Empty;
            }

            switch (fieldName.ToLowerInvariant())
            {
                case "firstname":
                    if (firstNameDictionary.TryGetValue(value, out var firstNameList))
                    {
                        return new ReadOnlyCollection<FileCabinetRecord>(firstNameList);
                    }
                    break;

                case "lastname":
                    if (lastNameDictionary.TryGetValue(value, out var lastNameList))
                    {
                        return new ReadOnlyCollection<FileCabinetRecord>(lastNameList);
                    }
                    break;

                case "dateofbirth":
                    if (dateOfBirthDictionary.TryGetValue(value, out var dateList))
                    {
                        return new ReadOnlyCollection<FileCabinetRecord>(dateList);
                    }
                    break;

                default:
                    throw new ArgumentException($"Field '{fieldName}' isn't supported", nameof(fieldName));
            }
            return ReadOnlyCollection<FileCabinetRecord>.Empty;
        }

    }
}
