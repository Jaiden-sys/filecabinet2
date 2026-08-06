using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using filecabinet;
using Moq;
namespace TestProject1
{
    public abstract class FileCabinetServiceContractTests
    {
        protected abstract IFileCabinetService CreateService();

        private static readonly RecordRequest ValidRecord = new(
            Id: 0,
            FirstName: "Test",
            LastName: "Jaiden",
            DateOfBirth: new DateTime(1950, 1, 2),
            ArchiveId: 12,
            Weight: 120m,
            Type: 'A'
        );

        [Fact]
        public void CreateRecord_ValidInput()
        {
            var service = CreateService();

            int id = service.CreateRecord(ValidRecord);

            var records = service.GetRecords();
            Assert.Single(records);

            var firstRecord = records[0];
            Assert.Equal(id, firstRecord.Id);
            Assert.Equal("Test", firstRecord.FirstName);
        }

        [Fact]
        public void CreateRecord_InvalidInputNullNames()
        {
            var service = CreateService();
            var invalidNameRecord = ValidRecord with { FirstName = null, LastName = null };

            Assert.Throws<ArgumentNullException>(() => service.CreateRecord(invalidNameRecord));
            Assert.Empty(service.GetRecords());
        }

        [Fact]
        public void CreateRecord_InvalidInputEmptyNames()
        {
            var service = CreateService();
            var emptyNameRecord = ValidRecord with { FirstName = " ", LastName = " " };

            Assert.Throws<ArgumentException>(() => service.CreateRecord(emptyNameRecord));
            Assert.Empty(service.GetRecords());
        }

        [Fact]
        public void CreateValidRecordTwice()
        {
            var service = CreateService();

            service.CreateRecord(ValidRecord);
            service.CreateRecord(ValidRecord);

            var records = service.GetRecords();
            Assert.Equal(2, records.Count);
            Assert.Equal(2, records[1].Id);
        }

        [Fact]
        public void EditRecord_ValidRequest_UpdatesFields()
        {
            var service = CreateService();
            int id = service.CreateRecord(ValidRecord);
            var updated = new RecordRequest(id, "Roman", "Petrov", new DateTime(1995, 5, 5), 2, 80m, 'B');

            service.EditRecord(updated);

            var record = service.GetRecords()[0];
            Assert.Equal(id, record.Id);
            Assert.Equal("Roman", record.FirstName);
            Assert.Equal("Petrov", record.LastName);
            Assert.Equal(new DateTime(1995, 5, 5), record.DateOfBirth);
            Assert.Equal(2, record.ArchiveId);
            Assert.Equal(80m, record.Weight);
            Assert.Equal('B', record.Type);
        }

        [Fact]
        public void EditRecord_NotExistingId_ThrowsArgumentException()
        {
            var service = CreateService();
            var request = ValidRecord with { Id = 999 };

            Assert.Throws<ArgumentException>(() => service.EditRecord(request));
        }

        [Fact]
        public void EditRecord_InvalidNewData_ThrowsAndKeepsOldData()
        {
            var service = CreateService();
            int id = service.CreateRecord(ValidRecord);
            var invalid = new RecordRequest(id, "R", "Petrov", new DateTime(1995, 5, 5), 2, 80m, 'B');

            Assert.Throws<ArgumentException>(() => service.EditRecord(invalid));

            var record = service.GetRecords()[0];
            Assert.Equal("Test", record.FirstName);
            Assert.Equal("Jaiden", record.LastName);
            Assert.Equal(new DateTime(1950, 1, 2), record.DateOfBirth);
            Assert.Equal(12, record.ArchiveId);
            Assert.Equal(120m, record.Weight);
            Assert.Equal('A', record.Type);
        }

        [Fact]
        public void EditRecord_NullRequest_ThrowsArgumentNullException()
        {
            var service = CreateService();

            Assert.Throws<ArgumentNullException>(() => service.EditRecord(null!));
        }

        [Fact]
        public void FindByField_Firstname()
        {
            var service = CreateService();
            service.CreateRecord(ValidRecord);

            var found = service.FindByField("firstname", ValidRecord.FirstName);

            Assert.Single(found);
        }

        [Fact]
        public void FindByField_Lastname()
        {
            var service = CreateService();
            service.CreateRecord(ValidRecord);

            var found = service.FindByField("lastname", ValidRecord.LastName);

            Assert.Single(found);
        }

        [Fact]
        public void FindByField_DateOfBirth()
        {
            var service = CreateService();
            service.CreateRecord(ValidRecord);

            var found = service.FindByField("dateofbirth", ValidRecord.DateOfBirth.ToString(CultureInfo.InvariantCulture));

            Assert.Single(found);
        }

        [Fact]
        public void FindByField_InvalidField()
        {
            var service = CreateService();
            service.CreateRecord(ValidRecord);

            Assert.Throws<ArgumentException>(() => service.FindByField("invalidfield", ValidRecord.FirstName));
        }

        [Fact]
        public void FindByField_WhiteSpacesField()
        {
            var service = CreateService();
            service.CreateRecord(ValidRecord);

            Assert.Throws<ArgumentNullException>(() => service.FindByField(" ", ValidRecord.LastName));
        }

        [Fact]
        public void FindByField_NullValue()
        {
            var service = CreateService();
            service.CreateRecord(ValidRecord);

            Assert.Empty(service.FindByField("firstname", null));
        }

        [Fact]
        public void FindByField_EmptyValue()
        {
            var service = CreateService();
            service.CreateRecord(ValidRecord);

            Assert.Empty(service.FindByField("firstname", " "));
        }

        [Fact]
        public void GetRecords_EmptyService_ReturnsEmptyCollection()
        {
            var service = CreateService();

            Assert.Empty(service.GetRecords());
        }

        [Fact]
        public void GetStat_EmptyService_ReturnsZero()
        {
            var service = CreateService();

            Assert.Equal(0, service.GetStat());
        }

        [Fact]
        public void GetRecords_AfterCreates_ReturnsAllRecordsInOrder()
        {
            var service = CreateService();

            service.CreateRecord(ValidRecord);
            service.CreateRecord(ValidRecord with { FirstName = "Roman" });

            var records = service.GetRecords();
            Assert.Equal(2, records.Count);
            Assert.Equal("Test", records[0].FirstName);
            Assert.Equal("Roman", records[1].FirstName);
        }

        [Fact]
        public void GetStat_AfterCreates_ReturnsRecordsCount()
        {
            var service = CreateService();

            service.CreateRecord(ValidRecord);
            service.CreateRecord(ValidRecord);
            service.CreateRecord(ValidRecord);

            Assert.Equal(3, service.GetStat());
        }
    }

    public class InMemoryFileCabinetServiceTests : FileCabinetServiceContractTests
    {
        protected override IFileCabinetService CreateService()
            => new FileCabinetService(new DefaultValidator());
    }

}
