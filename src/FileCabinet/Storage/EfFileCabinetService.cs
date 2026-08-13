using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;

namespace filecabinet
{
    public class EfFileCabinetService : IFileCabinetService
    {
        private readonly FileCabinetDbContext context;
        private readonly IRecordValidator validator;
        public EfFileCabinetService(IRecordValidator validator,FileCabinetDbContext context) 
        {this.validator = validator;
            this.context = context;
        }

        public int CreateRecord(RecordRequest request)
        {
            validator.ValidateParameters(request);
            var record = new FileCabinetRecord
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
                ArchiveId = request.ArchiveId,
                Weight = request.Weight,
                Type = request.Type
            };
            context.Records.Add(record); 
            context.SaveChanges();

            return record.Id;
        }
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            return context.Records.AsNoTracking().ToList().AsReadOnly();
        }
        public int GetStat()
        {
            if (context.Records.AsNoTracking().ToList().Count == 0) return 0;
            else return context.Records.AsNoTracking().ToList().AsReadOnly().Count;
        }
        private FileCabinetRecord? FindById(int id) => context.Records.Where(x => x.Id == id).FirstOrDefault();
        private FileCabinetRecord? FindByIdIncludingDeleted(int id) => context.Records.IgnoreQueryFilters().Where(x => x.Id == id).FirstOrDefault();
        public void EditRecord(RecordRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);
            this.validator.ValidateParameters(request);
            FileCabinetRecord? recordToUpdate = FindById(request.Id);
            if (recordToUpdate == null) throw new ArgumentException("Not found (id)");
            recordToUpdate.FirstName = request.FirstName;
            recordToUpdate.LastName = request.LastName;
            recordToUpdate.DateOfBirth = request.DateOfBirth;
            recordToUpdate.ArchiveId = request.ArchiveId;
            recordToUpdate.Weight = request.Weight;
            recordToUpdate.Type = request.Type;
            context.SaveChanges();
        }
        public ReadOnlyCollection<FileCabinetRecord> FindByField(string fieldName, string value)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
            {
                throw new ArgumentNullException(nameof(fieldName), "fieldname cannot be empty");
            }

            if (value == null)
            {
                return ReadOnlyCollection<FileCabinetRecord>.Empty;
            }
            switch (fieldName.ToLowerInvariant())
            {
                case "firstname":
                    return context.Records
                        .AsNoTracking()
                        .Where(r => r.FirstName == value)
                        .ToList()
                        .AsReadOnly();

                case "lastname":
                    return context.Records
                        .AsNoTracking()
                        .Where(r => r.LastName == value)
                        .ToList()
                        .AsReadOnly();

                case "dateofbirth":
                    if (!DateTime.TryParse(value, CultureInfo.InvariantCulture, out DateTime result)) 
                        return new List<FileCabinetRecord>().AsReadOnly();
                    return context.Records
                        .AsNoTracking()
                        .Where(r => r.DateOfBirth == result)
                        .ToList()
                        .AsReadOnly();

                default:
                    throw new ArgumentException($"Field '{fieldName}' isn't supported", nameof(fieldName));
            }
            
        }
        public void RemoveRecord(int id)
        {
            var foundRecord = FindById(id);
            if(foundRecord == null) throw new ArgumentException("Not found id");
            foundRecord.isDeleted = true;
            context.SaveChanges();
        }
        public void RestoreRecord(int id)
        {
            var foundRecord = FindByIdIncludingDeleted(id);
            if (foundRecord == null || foundRecord.isDeleted == false) throw new ArgumentException("Not found id or record with current id isn't deleted");
            foundRecord.isDeleted = false;
            context.SaveChanges();
        }
        
    }
}
