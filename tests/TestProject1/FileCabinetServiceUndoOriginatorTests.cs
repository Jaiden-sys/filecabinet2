using System;
using System.Collections.Generic;
using System.Text;
using filecabinet;
namespace TestProject1
{
    public class FileCabinetServiceUndoOriginatorTests
    {
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
        public void CreateMemento_ThenRestore_Reverts()
        {
            var service = new FileCabinetService(new DefaultValidator());
            int id = service.CreateRecord(ValidRecord);

            var memento = service.CreateMemento();
            service.RemoveRecord(id);
            Assert.Empty(service.GetRecords());

            service.Restore(memento);

            Assert.Single(service.GetRecords());
            Assert.Equal(id, service.GetRecords()[0].Id);
        }
    }
}
