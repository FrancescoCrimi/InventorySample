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
        private const string MainDbFileName = "app.db";
        private const string LogDbFileName = "log.db";
        private readonly IDatabaseConfigurationService _databaseConfigurationService;
        private readonly ISettingsService _settingsService;
        private const string DB_NAME = "VanArsdel";
        private const string DB_VERSION = "1.02";

        private static readonly string AppLogPath = "AppLog";
        private static readonly string AppLogName = $"AppLog.1.0.db";
        private static readonly string AppLogFileName = Path.Combine(AppLogPath, AppLogName);

        private static readonly string DatabasePath = "Database";
        private static readonly string DatabaseName = $"{DB_NAME}.{DB_VERSION}.db";
        private static readonly string DatabaseFileName = Path.Combine(DatabasePath, DatabaseName);

        private readonly ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;
        private readonly StorageFolder _localFolder = ApplicationData.Current.LocalFolder;


        private readonly string AppLogConnectionString = $"Data Source={AppLogFileName}";

        private readonly string SQLiteConnectionString = $"Data Source={DatabaseFileName}";


        public LocalDatabaseProvisioner(IDatabaseConfigurationService databaseConfigurationService,
                                        ISettingsService settingsService)
        {
            _databaseConfigurationService = databaseConfigurationService;
            _settingsService = settingsService;
        }

        public async Task EnsureMainDatabaseAsync( CancellationToken ct = default)
        {
            //var localFolder = ApplicationData.Current.LocalFolder;
            var localFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(DatabasePath, CreationCollisionOption.OpenIfExists);

            //var dbPath = Path.Combine(localFolder.Path, MainDbFileName);
            var dbPath = Path.Combine(localFolder.Path, DatabaseName);
            if (!File.Exists(dbPath))
            {

                //var assetPath = Path.Combine(Package.Current.InstalledLocation.Path, "Assets", MainDbFileName);
                var assetPath = Path.Combine(Package.Current.InstalledLocation.Path, "Assets", DatabasePath, DatabaseName);

                var sourceFile = await StorageFile.GetFileFromPathAsync(assetPath);

                await sourceFile.CopyAsync(localFolder, DatabaseName, NameCollisionOption.ReplaceExisting);

            }

            // 🔒 3. Registra profilo SQLite se non esiste
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

        //public async Task EnsureLocalDatabaseAsync()
        //{
        //    var databaseFolder = await _localFolder.CreateFolderAsync(DatabasePath, CreationCollisionOption.OpenIfExists);
        //    if (await databaseFolder.TryGetItemAsync(DatabaseName) == null)
        //    {
        //        var sourceFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Database/VanArsdel.1.02.db"));
        //        var targetFile = await databaseFolder.CreateFileAsync(DatabaseName, CreationCollisionOption.ReplaceExisting);
        //        await sourceFile.CopyAndReplaceAsync(targetFile);
        //    }
        //}

        public async Task EnsureLogDatabaseAsync(CancellationToken ct = default)
        {
            //var localFolder = ApplicationData.Current.LocalFolder;
            var localFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(AppLogPath, CreationCollisionOption.OpenIfExists);
            var dbPath = Path.Combine(localFolder.Path, AppLogName);
            if (File.Exists(dbPath)) return;

            //var assetPath = Path.Combine(Package.Current.InstalledLocation.Path, "Assets", LogDbFileName);
            var assetPath = Path.Combine(Package.Current.InstalledLocation.Path, "Assets", AppLogPath, AppLogName);

            var sourceFile = await StorageFile.GetFileFromPathAsync(assetPath);
            await sourceFile.CopyAsync(localFolder, AppLogName, NameCollisionOption.ReplaceExisting);
        }

        //public async Task EnsureLogDatabaseAsync()
        //{
        //    var appLogFolder = await _localFolder.CreateFolderAsync(AppLogPath, CreationCollisionOption.OpenIfExists);
        //    if (await appLogFolder.TryGetItemAsync(AppLogName) == null)
        //    {
        //        var sourceLogFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/AppLog/AppLog.db"));
        //        var targetLogFile = await appLogFolder.CreateFileAsync(AppLogName, CreationCollisionOption.ReplaceExisting);
        //        await sourceLogFile.CopyAndReplaceAsync(targetLogFile);
        //    }
        //}

        public string DatabaseLoggerConnectionString => $"Data Source={AppLogFileName}";
    }
}
