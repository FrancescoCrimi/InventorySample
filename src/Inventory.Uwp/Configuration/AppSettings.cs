// Copyright (c) Microsoft. All rights reserved.
// Copyright (c) 2023 Francesco Crimi francrim@gmail.com
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.

using Inventory.Infrastructure;
using Inventory.Infrastructure.Common;
using Inventory.Uwp.Common;
using Inventory.Uwp.Helpers;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace Inventory.Uwp
{
    public class AppSettings : IAppSettings
    {
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


        public string AppLogConnectionString => $"Data Source={AppLogFileName}";

        public string SQLiteConnectionString => $"Data Source={DatabaseFileName}";

        public string Version
        {
            get
            {
                var ver = Package.Current.Id.Version;
                return $"{ver.Major}.{ver.Minor}.{ver.Build}.{ver.Revision}";
            }
        }

        public string DbVersion => DB_VERSION;

        public DataProviderType DataProvider
        {
            get => (DataProviderType)_localSettings.ReadInt("DataProvider", (int)DataProviderType.SQLite);
            set => _localSettings.SaveInt("DataProvider", (int)value);
        }

        public string SQLServerConnectionString
        {
            get => _localSettings.ReadString("SQLServerConnectionString", @"Data Source=.\SQLExpress;Initial Catalog=VanArsdelDb;Integrated Security=SSPI");
            set => _localSettings.SaveString("SQLServerConnectionString", value);
        }

        public bool IsRandomErrorsEnabled
        {
            get => _localSettings.ReadBoolean("IsRandomErrorsEnabled", false);
            set => _localSettings.SaveBoolean("IsRandomErrorsEnabled", value);
        }


        public async Task<Result> ResetLocalDatabaseAsync()
        {
            Result result;
            try
            {
                var databaseFolder = await _localFolder.CreateFolderAsync(DatabasePath, CreationCollisionOption.OpenIfExists);
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

        public async Task EnsureLocalDatabaseAsync()
        {
            var databaseFolder = await _localFolder.CreateFolderAsync(DatabasePath, CreationCollisionOption.OpenIfExists);
            if (await databaseFolder.TryGetItemAsync(DatabaseName) == null)
            {
                var sourceFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Database/VanArsdel.1.02.db"));
                var targetFile = await databaseFolder.CreateFileAsync(DatabaseName, CreationCollisionOption.ReplaceExisting);
                await sourceFile.CopyAndReplaceAsync(targetFile);
            }
        }

        public async Task EnsureLogDatabaseAsync()
        {
            var appLogFolder = await _localFolder.CreateFolderAsync(AppLogPath, CreationCollisionOption.OpenIfExists);
            if (await appLogFolder.TryGetItemAsync(AppLogName) == null)
            {
                var sourceLogFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/AppLog/AppLog.db"));
                var targetLogFile = await appLogFolder.CreateFileAsync(AppLogName, CreationCollisionOption.ReplaceExisting);
                await sourceLogFile.CopyAndReplaceAsync(targetLogFile);
            }
        }
    }
}
