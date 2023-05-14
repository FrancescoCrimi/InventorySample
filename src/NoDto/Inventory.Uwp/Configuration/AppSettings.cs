#region copyright
// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************
#endregion

using Inventory.Infrastructure;
using Inventory.Infrastructure.Common;
using Inventory.Uwp.Helpers;
using System.IO;
using Windows.ApplicationModel;
using Windows.Storage;

namespace Inventory.Uwp
{
    public class AppSettings : IAppSettings
    {
        private const string DB_NAME = "VanArsdel";
        private const string DB_VERSION = "1.02";
        private const string DB_BASEURL = "https://vanarsdelinventory.blob.core.windows.net/database";

        public static readonly string AppLogPath = "AppLog";
        public static readonly string AppLogName = $"AppLog.1.0.db";
        public static readonly string AppLogFileName = Path.Combine(AppLogPath, AppLogName);

        public static readonly string DatabasePath = "Database";
        public static readonly string DatabaseName = $"{DB_NAME}.{DB_VERSION}.db";
        public static readonly string DatabasePattern = $"{DB_NAME}.{DB_VERSION}.pattern.db";
        public static readonly string DatabaseFileName = Path.Combine(DatabasePath, DatabaseName);
        public static readonly string DatabasePatternFileName = Path.Combine(DatabasePath, DatabasePattern);
        public static readonly string DatabaseUrl = $"{DB_BASEURL}/{DatabaseName}";

        static AppSettings()
        {
            Current = new AppSettings();
        }

        public static AppSettings Current { get; }

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
            get => (DataProviderType)LocalSettings.ReadInt("DataProvider", (int)DataProviderType.SQLite);
            set => LocalSettings.SaveInt("DataProvider", (int)value);
        }

        public string SQLServerConnectionString
        {
            get => LocalSettings.ReadString("SQLServerConnectionString", @"Data Source=.\SQLExpress;Initial Catalog=VanArsdelDb;Integrated Security=SSPI");
            set => LocalSettings.SaveString("SQLServerConnectionString", value);
        }

        public bool IsRandomErrorsEnabled
        {
            get => LocalSettings.ReadBoolean("IsRandomErrorsEnabled", false);
            set => LocalSettings.SaveBoolean("IsRandomErrorsEnabled", value);
        }


        private ApplicationDataContainer LocalSettings => ApplicationData.Current.LocalSettings;
    }
}
