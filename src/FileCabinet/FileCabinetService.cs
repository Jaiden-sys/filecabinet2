using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace filecabinet
{
    public class FileCabinetService : IFileCabinetService, IUndoOriginator
    {
        private readonly List<FileCabinetRecord> _list = new List<FileCabinetRecord>();
        protected readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        protected readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        protected readonly Dictionary<string, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        protected readonly DateTime limitedDateOfBirth = new DateTime(1950, 1, 1);
        protected readonly CultureInfo culture = CultureInfo.InvariantCulture;
        protected IRecordValidator validator;
        public FileCabinetService(IRecordValidator validator) { this.validator = validator; }
        private int _nextId = 1;
        private static void AddToIndex(
            Dictionary<string, List<FileCabinetRecord>> dictionary,
            string key,
            FileCabinetRecord record)
        {
            if (!dictionary.TryGetValue(key, out var list))
            {
                list = new List<FileCabinetRecord>();
                dictionary.Add(key, list);
            }
            list.Add(record);
        }

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
                Id = this._nextId++,
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
                ArchiveId = request.ArchiveId,
                Weight = request.Weight,
                Type = request.Type
            };

            _list.Add(record);
            
            AddToIndex(firstNameDictionary, request.FirstName, record);
            AddToIndex(lastNameDictionary, request.LastName, record);
            AddToIndex(dateOfBirthDictionary, request.DateOfBirth.ToString(culture), record);
            

            return record.Id;
        }
        /// <summary>
        /// Returns array with records or empty array
        /// </summary>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            return _list.Where(x => !x.isDeleted).ToList().AsReadOnly();
        }
        /// <summary>
        /// Shows quantity of records
        /// </summary>
        /// <returns></returns>
        public int GetStat()
        {
            if (_list.Count == 0) return 0;
            else return _list.Where(x => !x.isDeleted).ToList().AsReadOnly().Count;
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
        private FileCabinetRecord? FindById(int id) => _list.FirstOrDefault(x => (x.Id == id && x.isDeleted == false));
        public void EditRecord(RecordRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);
            this.validator.ValidateParameters(request);
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
                        return firstNameList.Where(x => !x.isDeleted).ToList().AsReadOnly();
                    }
                    break;

                case "lastname":
                    if (lastNameDictionary.TryGetValue(value, out var lastNameList))
                    {
                        return lastNameList.Where(x => !x.isDeleted).ToList().AsReadOnly();
                    }
                    break;

                case "dateofbirth":
                    if (dateOfBirthDictionary.TryGetValue(value, out var dateList))
                    {
                        return dateList.Where(x => !x.isDeleted).ToList().AsReadOnly();
                    }
                    break;

                default:
                    throw new ArgumentException($"Field '{fieldName}' isn't supported", nameof(fieldName));
            }
            return ReadOnlyCollection<FileCabinetRecord>.Empty;
        }
        public void RemoveRecord(int id)
        {
            var foundRecord = FindById(id);
            if (foundRecord == null) 
            {
                throw new ArgumentException("Not found id");
            }
            foundRecord.isDeleted = true;
        }
        
        public IMemento CreateMemento()
        {
            var snapshot = _list.Select(r => new FileCabinetRecord
            {
                Id = r.Id,
                FirstName = r.FirstName,
                LastName = r.LastName,
                DateOfBirth = r.DateOfBirth,
                ArchiveId = r.ArchiveId,
                Weight = r.Weight,
                Type = r.Type,
                isDeleted = r.isDeleted,
            }).ToList();
            return new ConcreteMemento(snapshot);
        }
        public void Restore(IMemento memento)
        {
            if (memento is not ConcreteMemento concreteMememento)
            {
                throw new ArgumentException("Unknown memento type.", nameof(memento));
            }

            _list.Clear();
            _list.AddRange(concreteMememento.State);
            RebuildIndices();
        }
        private void RebuildIndices()
        {
            firstNameDictionary.Clear();
            lastNameDictionary.Clear();
            dateOfBirthDictionary.Clear();

            foreach (var record in _list)
            {
                AddToIndex(firstNameDictionary, record.FirstName, record);
                AddToIndex(lastNameDictionary, record.LastName, record);
                AddToIndex(dateOfBirthDictionary, record.DateOfBirth.ToString(culture), record);
            }
        }
        private sealed class ConcreteMemento : IMemento
        {
            public List<FileCabinetRecord> State { get; }
            public ConcreteMemento(List<FileCabinetRecord> state) => State = state;
        }
    }
}
