using System;
using System.Collections.Generic;
using System.Text;
using filecabinet;
using Moq;
namespace TestProject1
{
    public class ServiceTest
    {
        private readonly Mock<IRecordValidator> _validatorMock;
        private readonly FileCabinetService _service;

        public ServiceTest()
        {
            _validatorMock = new Mock<IRecordValidator>();
            _validatorMock.Setup( v => v.ValidateParameters(It.IsAny<RecordRequest>()))
                .Verifiable();
            _service = new FileCabinetService(_validatorMock.Object);
        }

        [Fact]
        public void CreateRecord_ValidInput()
        {
            //Arrange
            var record = new RecordRequest(Id: 1,
                FirstName: "Test",
                LastName: "Jaiden",
                DateOfBirth: new DateTime(1950, 01, 02),
                ArchiveId: 12,
                Weight: 120,
                Type: 'A');

            //Act

            _service.CreateRecord(record);

            //Assert
            _validatorMock.Verify(v => v.ValidateParameters(record), Times.Once);

            var records = _service.GetRecords();

            Assert.Single(records);
            

            var firstrecord = records.First();
            Assert.Equal(1, firstrecord.Id);
            Assert.Equal("Test", firstrecord.FirstName);
        }
        /*
        [Fact]
        
        public void CreateRecord_InvalidInput() 
        {
            //Arrange
            var invalidRecord = new RecordRequest(
                Id: -255,
                FirstName: " ",
                LastName: "Z",
                DateOfBirth: new DateTime(1945, 09, 09),
                ArchiveId: -1,
                Weight: -2,
                Type: ' ');
            _validatorMock
                .Setup(v => v.ValidateParameters(invalidRecord))
                .Throws(new )
            //Act
            _service.CreateRecord(invalidRecord);
        }
        */

    }
}
