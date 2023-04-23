using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Inventory.Infrastructure.Logging
{
    public class LogDbContext : DbContext
    {
        public LogDbContext(DbContextOptions<LogDbContext> options)
            : base(options)
        {
        }

        protected LogDbContext()
        {
        }

        public virtual DbSet<Log> Logs
        {
            get; set;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Log>(log =>
            {
                log.ToTable(@"logs");
                log.Property(x => x.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();
                log.HasKey(x => x.Id);
                //entity.Property(x => x.MachineName)
                //.IsRequired();
                //entity.Property(x => x.Logged)
                //.IsRequired();
                log.Property(x => x.Level)
                .IsRequired();
                log.Property(x => x.Message)
                .IsRequired();
                log.Property(x => x.DateTime)
                .HasConversion(new DateTimeOffsetToBinaryConverter());
            });
        }
    }
}
