using Inventory.Infrastructure.Settings;
using Inventory.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Persistence
{
    public class AppDbContextFactory
    {
        private readonly IDatabaseConfigurationService _dbSettings;

        public AppDbContextFactory(IDatabaseConfigurationService dbSettings) =>
            _dbSettings = dbSettings;

        public AppDbContext CreateDbContext()
        {
            AppDbContext appDbContext = null;
            var config = _dbSettings.GetCurrent();

            switch (config.Provider)
            {
                case DatabaseProviderType.SQLite:
                    var sQLiteBuilder = new DbContextOptionsBuilder<SQLiteAppDbContext>();
                    sQLiteBuilder
                        .UseSqlite(config.ConnectionString)
                        .EnableSensitiveDataLogging(true);
                    appDbContext = new SQLiteAppDbContext(sQLiteBuilder.Options);
                    break;
                case DatabaseProviderType.SQLServer:
                    var sQLServerBuilder = new DbContextOptionsBuilder<SQLServerAppDbContext>();
                    sQLServerBuilder.UseSqlServer(config.ConnectionString);
                    appDbContext = new SQLServerAppDbContext(sQLServerBuilder.Options);
                    break;
                    //case DatabaseProviderType.MySQL:
                    //    var builder = new DbContextOptionsBuilder<MySqlAppDbContext>();
                    //    builder.UseMySql(config.ConnectionString, ServerVersion.AutoDetect(config.ConnectionString));
                    //    break;
                    //case DatabaseProviderType.PostgreSQL:
                    //    builder.UseNpgsql(config.ConnectionString);
                    //    break;
            }

            return appDbContext;
        }
    }
}
