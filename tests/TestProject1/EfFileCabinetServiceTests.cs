using filecabinet;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestProject1
{
    //TODO: Implement the creation of the database and schema once for the entire
    //test class using IClassFixture and clean the tables using DELETE FROM Records
    public class EfFileCabinetServiceTests : FileCabinetServiceContractTests
    {
        private const string testConnectionString =
            "Server=(localdb)\\MSSQLLocalDB;Database=FileCabinetTests;Trusted_Connection=True;TrustServerCertificate=True;";
        
        private EfFileCabinetService CreateEfService()
        {
            var options = new DbContextOptionsBuilder<FileCabinetDbContext>()
            .UseSqlServer(testConnectionString)
            .Options;

            var dbContext = new FileCabinetDbContext(options);
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
            return new EfFileCabinetService(new DefaultValidator(), dbContext);
        }
        protected override IFileCabinetService CreateService() => CreateEfService();

        [Fact]
        public void Restore_Deleted_Record()
        {
            var service = CreateEfService();
            
            int id = service.CreateRecord(ValidRecord);
            service.RemoveRecord(id);
            service.RestoreRecord(id);
            Assert.Equal(1, service.GetStat());
        }
        [Fact]
        public void Restore_nonDeleted_Record()
        {
            var service = CreateEfService();
            int id = service.CreateRecord(ValidRecord);
            Assert.Throws<ArgumentException>(() => service.RestoreRecord(id));
        }
    }
}
