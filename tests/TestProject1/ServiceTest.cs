using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using filecabinet;
using Moq;
namespace TestProject1
{
    public class ServiceTest
    {
        private readonly IFileCabinetService _service = new FileCabinetService(new DefaultValidator());
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
            // Act
            int id = _service.CreateRecord(ValidRecord);

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
            var invalidNameRecord = ValidRecord with { FirstName = null, LastName = null };

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _service.CreateRecord(invalidNameRecord));

            var records = _service.GetRecords();
            Assert.Empty(records);
        }
        [Fact]
        public void CreateRecord_InvalidInputEmptyNames()
        {
            // Arrange
            var emptyNameRecord = ValidRecord with { FirstName = " ", LastName = " " };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _service.CreateRecord(emptyNameRecord));

            var records = _service.GetRecords();
            Assert.Empty(records);
        }

        [Fact]
        public void CreateValidRecordTwice()
        {
            // Act
            _service.CreateRecord(ValidRecord);
            _service.CreateRecord(ValidRecord);

            // Assert
            var records = _service.GetRecords();
            Assert.Equal(2, records.Count);
            Assert.Equal(2, records[1].Id);
        }

        [Fact]
        public void EditRecord_ValidRequest_UpdatesFields()
        {
            // Arrange
            int id = _service.CreateRecord(ValidRecord);
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
            var request = ValidRecord with { Id = 999 };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _service.EditRecord(request));
        }

        [Fact]
        public void EditRecord_InvalidNewData_ThrowsAndKeepsOldData()
        {
            // Arrange
            int id = _service.CreateRecord(ValidRecord);
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
        /*
неподдерживаемое поле → ArgumentException
null/пробелы в fieldName → ArgumentNullException
null value → пустая коллекция
совпадений нет → пустая коллекция
GetRecords / GetStat:
пустой сервис → пустая коллекция / 0
после созданий → все записи / верное число
2. Тесты конвертеров ConsoleReader
Это чистые функции, тестируются без консоли:
StringConverter: " abc " → (true, "abc"); "" и null → false
DecimalConverter: "70.5" → true; "abc" → false
DateTimeConverter: "1990-01-01" → true; "текст" → false
ShortConverter: "5" → true; "abc" → false
CharConverter: "A" → true; "AB" → false
         */

        [Fact]
        public void FindByField_Firstname()
        {
            //Arrange
            _service.CreateRecord(ValidRecord);

            //Act
            var foundRecord = _service.FindByField("firstname", ValidRecord.FirstName);

            //Assert
            Assert.Single(foundRecord);
        }
        [Fact]
        public void FindByField_Lastname()
        {
            //Arrange
            _service.CreateRecord(ValidRecord);

            //Act
            var foundRecord = _service.FindByField("lastname", ValidRecord.LastName);

            //Assert
            Assert.Single(foundRecord);
        }
        [Fact]
        public void FindByField_dateofbirth()
        {
            //Arrange
            _service.CreateRecord(ValidRecord);

            //Act
            var foundRecord = _service.FindByField("dateofbirth", ValidRecord.DateOfBirth.ToString(CultureInfo.InvariantCulture));

            //Assert
            Assert.Single(foundRecord);
        }
        [Fact]
        public void FindByField_invalidField()
        {
            //Arrange
            _service.CreateRecord(ValidRecord);

            //Act and Assert
            Assert.Throws<ArgumentException>( () => _service.FindByField("invalidfield", ValidRecord.FirstName));
        }
        [Fact]
        public void FindByField_whiteSpacesField()
        {
            //Arrange
            _service.CreateRecord(ValidRecord);
            //Act and Assert
            Assert.Throws<ArgumentNullException>(() => _service.FindByField(" ",ValidRecord.LastName));
        }
        [Fact]
        public void FindByField_nullValue()
        {
            //Arrange
            _service.CreateRecord(ValidRecord);
            //Act and Assert
            Assert.Empty(_service.FindByField("firstname", null));
        }
        [Fact]
        public void FindByField_emptyvalue()
        {
            //Arrange
            _service.CreateRecord(ValidRecord);
            //Act and Assert
            Assert.Empty(_service.FindByField("firstname", " "));
        }
        [Fact]
        public void GetRecords_EmptyService_ReturnsEmptyCollection()
        {
            Assert.Empty(_service.GetRecords());
        }

        [Fact]
        public void GetStat_EmptyService_ReturnsZero()
        {
            Assert.Equal(0, _service.GetStat());
        }

        [Fact]
        public void GetRecords_AfterCreates_ReturnsAllRecordsInOrder()
        {
            // Act
            _service.CreateRecord(ValidRecord);
            _service.CreateRecord(ValidRecord with { FirstName = "Roman" });

            // Assert
            var records = _service.GetRecords();
            Assert.Equal(2, records.Count);
            Assert.Equal("Test", records[0].FirstName);
            Assert.Equal("Roman", records[1].FirstName);
        }

        [Fact]
        public void GetStat_AfterCreates_ReturnsRecordsCount()
        {
            // Act
            _service.CreateRecord(ValidRecord);
            _service.CreateRecord(ValidRecord);
            _service.CreateRecord(ValidRecord);

            // Assert
            Assert.Equal(3, _service.GetStat());
        }
    }
}
