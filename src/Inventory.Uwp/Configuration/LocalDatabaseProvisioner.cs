using Inventory.Infrastructure.Common;
using Inventory.Infrastructure.Settings;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace Inventory.Uwp.Configuration
{
    public class LocalDatabaseProvisioner : ILocalDatabaseProvisioner
    {
        private const string DB_NAME = "VanArsdel";
        private const string DB_VERSION = "1.02";

        private const string DatabasePath = "Database";
        private static readonly string DatabaseName = $"{DB_NAME}.{DB_VERSION}.db";
        private static readonly string DatabaseFileName = Path.Combine(DatabasePath, DatabaseName);

        private const string AppLogPath = "AppLog";
        private const string AppLogName = "AppLog.1.0.db";
        private static readonly string AppLogFileName = Path.Combine(AppLogPath, AppLogName);

        private readonly string AppLogConnectionString = $"Data Source={AppLogFileName}";
        private readonly string SQLiteConnectionString = $"Data Source={DatabaseFileName}";

        private readonly IDatabaseConfigurationService _databaseConfigurationService;

        public LocalDatabaseProvisioner(IDatabaseConfigurationService databaseConfigurationService)
            => _databaseConfigurationService = databaseConfigurationService;

        public async Task EnsureMainDatabaseAsync(CancellationToken ct = default)
        {
            var localFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(DatabasePath, CreationCollisionOption.OpenIfExists);
            var dbPath = Path.Combine(localFolder.Path, DatabaseName);
            if (!File.Exists(dbPath))
            {
                var assetPath = Path.Combine(Package.Current.InstalledLocation.Path, "Assets", DatabasePath, DatabaseName);
                var sourceFile = await StorageFile.GetFileFromPathAsync(assetPath);
                await sourceFile.CopyAsync(localFolder, DatabaseName, NameCollisionOption.ReplaceExisting);
            }

            // Registra profilo SQLite se non esiste
            if (!_databaseConfigurationService.GetAll().Any(c => c.Key == "DefaultSQLite"))
            {
                _databaseConfigurationService.Add(new DatabaseConfiguration
                (
                    key: "DefaultSQLite",
                    provider: DatabaseProviderType.SQLite,
                    connectionString: SQLiteConnectionString,
                    isReadOnly: true
                ));
                _databaseConfigurationService.SetCurrent("DefaultSQLite");
            }
        }

        public async Task EnsureLogDatabaseAsync(CancellationToken ct = default)
        {
            var localFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(AppLogPath, CreationCollisionOption.OpenIfExists);
            var dbPath = Path.Combine(localFolder.Path, AppLogName);
            if (File.Exists(dbPath)) return;

            var assetPath = Path.Combine(Package.Current.InstalledLocation.Path, "Assets", AppLogPath, AppLogName);
            var sourceFile = await StorageFile.GetFileFromPathAsync(assetPath);
            await sourceFile.CopyAsync(localFolder, AppLogName, NameCollisionOption.ReplaceExisting);
        }

        public async Task<Result> ResetLocalDatabaseAsync()
        {
            Result result;
            try
            {
                var databaseFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(DatabasePath, CreationCollisionOption.OpenIfExists);
                var sourceFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Database/VanArsdel.1.01.db"));
                var targetFile = await databaseFolder.CreateFileAsync(DatabaseName, CreationCollisionOption.ReplaceExisting);
                await sourceFile.CopyAndReplaceAsync(targetFile);
                result = Result.Ok();
            }
            catch (Exception ex)
            {
                result = Result.Error(ex);
            }
            return result;
        }

        public string DatabaseLoggerConnectionString => AppLogConnectionString;
    }
}
