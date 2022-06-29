using CiccioSoft.Inventory.Data;
using Microsoft.EntityFrameworkCore;

namespace CiccioSoft.Inventory.Persistence.DbContexts
{
    public class LogDbContext : DbContext
    {
        public LogDbContext(DbContextOptions<LogDbContext> options) : base(options) { }

        protected LogDbContext() { }

        public virtual DbSet<Log> Logs { get; set; }

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
