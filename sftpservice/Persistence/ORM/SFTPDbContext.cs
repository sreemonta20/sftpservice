using Microsoft.EntityFrameworkCore;
using sftpservice.Core.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sftpservice.Persistence.ORM
{
    /// <summary>
    /// It acts as the FTP database context of Entity Framework Core for the Postgresql database. 
    /// </summary>
    public class SFTPDbContext : DbContext
    {
        public SFTPDbContext(DbContextOptions<SFTPDbContext> options) : base(options)
        {

        }
        public DbSet<FileDetails> FileDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<FileDetails>().Property(e => e.Id).ValueGeneratedOnAdd();


        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging(true);
        }
    }
}
