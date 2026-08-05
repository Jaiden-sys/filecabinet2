using System;
using System.Collections.Generic;
using System.Text;
using filecabinet;
using Moq;
namespace TestProject1
{
    public class ServiceTest
    {
        private readonly FileCabinetService _service;
        private readonly RecordRequest _validRecord;

        public ServiceTest()
        {
            _validRecord = new RecordRequest(
                Id: 0,
                FirstName: "Test",
                LastName: "Jaiden",
                DateOfBirth: new DateTime(1950, 1, 2),
                ArchiveId: 12,
                Weight: 120m,
                Type: 'A'
            );
            _service = new FileCabinetService(new DefaultValidator());
        }

        [Fact]
        public void CreateRecord_ValidInput()
        {
            // Act
            int id = _service.CreateRecord(_validRecord);

            // Assert
            var records = _service.GetRecords();
            Assert.Single(records);

            var firstRecord = records[0];
            Assert.Equal(id, firstRecord.Id);
            Assert.Equal("Test", firstRecord.FirstName);
        }

        [Fact]
        public void CreateRecord_InvalidInputNullNames()
        {
            // Arrange
            var invalidNameRecord = _validRecord with { FirstName = null, LastName = null };

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _service.CreateRecord(invalidNameRecord));

            var records = _service.GetRecords();
            Assert.Empty(records);
        }
        [Fact]
        public void CreateRecord_InvalidInputEmptyNames()
        {
            // Arrange
            var emptyNameRecord = _validRecord with { FirstName = " ", LastName = " " };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _service.CreateRecord(emptyNameRecord));

            var records = _service.GetRecords();
            Assert.Empty(records);
        }

        [Fact]
        public void CreateValidRecordTwice()
        {
            // Act
            _service.CreateRecord(_validRecord);
            _service.CreateRecord(_validRecord);

            // Assert
            var records = _service.GetRecords();
            Assert.Equal(2, records.Count);
            Assert.Equal(2, records[1].Id);
        }

        [Fact]
        public void EditRecord_ValidRequest_UpdatesFields()
        {
            // Arrange
            int id = _service.CreateRecord(_validRecord);
            var updated = new RecordRequest(id, "Roman", "Petrov", new DateTime(1995, 5, 5), 2, 80m, 'B');

            // Act
            _service.EditRecord(updated);

            // Assert
            var records = _service.GetRecords();
            Assert.Single(records);

            var record = records[0];
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
            // Arrange
            var request = _validRecord with { Id = 999 };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _service.EditRecord(request));
        }

        [Fact]
        public void EditRecord_InvalidNewData_ThrowsAndKeepsOldData()
        {
            // Arrange
            int id = _service.CreateRecord(_validRecord);
            var invalid = new RecordRequest(id, "R", "Petrov", new DateTime(1995, 5, 5), 2, 80m, 'B');

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _service.EditRecord(invalid));

            var record = _service.GetRecords()[0];
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
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _service.EditRecord(null!));
        }

    }
}
