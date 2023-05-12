using Inventory.Infrastructure;
using Inventory.Infrastructure.Common;

namespace ConsoleApp
{
    internal class AppSettings : IAppSettings
    {
        public string DbVersion { get; set; }

        public bool IsRandomErrorsEnabled { get; set; }

        public string Version { get; set; }

        public DataProviderType DataProvider { get; set; }

        public string SQLiteConnectionString { get; set; }

        public string SQLServerConnectionString { get; set; }

        public string AppLogConnectionString { get; set; }
    }
}