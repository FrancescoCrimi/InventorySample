using Inventory.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Logging
{
    public class LogDbContextFactory
    {
        private readonly ILocalDatabaseProvisioner _localDatabaseProvisioner;

        public LogDbContextFactory(ILocalDatabaseProvisioner localDatabaseProvisioner)
            => _localDatabaseProvisioner = localDatabaseProvisioner;

        public LogDbContext CreateDbContext()
        {
            var builder = new DbContextOptionsBuilder<LogDbContext>()
                .UseSqlite(_localDatabaseProvisioner.DatabaseLoggerConnectionString);
                //.EnableSensitiveDataLogging(true);
            LogDbContext loggerDbContext = new LogDbContext(builder.Options);
            return loggerDbContext;
        }
    }
}
