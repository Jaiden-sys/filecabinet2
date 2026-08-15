using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace filecabinet
{
    public class FileCabinetDbContextFactory : IDesignTimeDbContextFactory<FileCabinetDbContext>
    {
        public FileCabinetDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<FileCabinetDbContext>()
            .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=FileCabinet;Trusted_Connection=True;TrustServerCertificate=True;")
            .Options;
            return new FileCabinetDbContext(options);
        }
    }
}
