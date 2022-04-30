using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CiccioSoft.Inventory.Data.DbContexts
{
    public class LogDbContext : DbContext, IDisposable
    {
        //private readonly string connectionString;

        //public LogDbContext(string connectionString)
        //    : base()
        //{
        //    this.connectionString = connectionString;
        //}

        public LogDbContext(DbContextOptions<LogDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Log> Logs { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder
        //        .UseSqlite(connectionString);
        //    base.OnConfiguring(optionsBuilder);
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Log>()
                .ToTable(@"log");

            modelBuilder.Entity<Log>()
                .Property(x => x.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Log>()
                .Property(x => x.MachineName)
                .IsRequired();

            modelBuilder.Entity<Log>()
                .Property(x => x.Logged)
                .IsRequired();

            modelBuilder.Entity<Log>()
                .Property(x => x.Level)
                .IsRequired();

            modelBuilder.Entity<Log>()
                .Property(x => x.Message)
                .IsRequired();

            modelBuilder.Entity<Log>()
                .HasKey(@"Id");
        }
    }
}
