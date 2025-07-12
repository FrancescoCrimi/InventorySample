using Inventory.Infrastructure.Settings;
using System;
using System.IO;
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

        public async Task EnsureMainDatabaseAsync(DatabaseConfiguration config, CancellationToken ct = default)
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            var dbPath = Path.Combine(localFolder.Path, MainDbFileName);
            if (File.Exists(dbPath)) return;

            var assetPath = Path.Combine(Package.Current.InstalledLocation.Path, "Assets", MainDbFileName);
            var sourceFile = await StorageFile.GetFileFromPathAsync(assetPath);
            await sourceFile.CopyAsync(localFolder, MainDbFileName, NameCollisionOption.ReplaceExisting);
        }

        public async Task EnsureLogDatabaseAsync(CancellationToken ct = default)
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            var dbPath = Path.Combine(localFolder.Path, LogDbFileName);
            if (File.Exists(dbPath)) return;

            var assetPath = Path.Combine(Package.Current.InstalledLocation.Path, "Assets", LogDbFileName);
            var sourceFile = await StorageFile.GetFileFromPathAsync(assetPath);
            await sourceFile.CopyAsync(localFolder, LogDbFileName, NameCollisionOption.ReplaceExisting);
        }
    }
}
