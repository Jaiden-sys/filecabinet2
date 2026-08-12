using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
namespace filecabinet
{
    public class FileCabinetDbContext : DbContext
    {
        public FileCabinetDbContext(DbContextOptions<FileCabinetDbContext> options) : base(options) 
        { 
        
        }
        public DbSet<FileCabinetRecord> Records => Set<FileCabinetRecord>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileCabinetRecord>()
                .HasQueryFilter(r => !r.IsDeleted);
        }

    }

}
