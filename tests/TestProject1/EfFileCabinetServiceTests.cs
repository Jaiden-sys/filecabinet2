using filecabinet;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestProject1
{
    public class EfFileCabinetServiceTests : FileCabinetServiceContractTests
    {
        private const string testConnectionString =
            "Server=(localdb)\\MSSQLLocalDB;Database=FileCabinetTests;Trusted_Connection=True;TrustServerCertificate=True;";
        protected override IFileCabinetService CreateService()
        {
            var options = new DbContextOptionsBuilder<FileCabinetDbContext>()
            .UseSqlServer(testConnectionString)
            .Options;

            var dbContext = new FileCabinetDbContext(options);
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
            return new EfFileCabinetService(new DefaultValidator(), dbContext);
        }
    }
}
