using System;
using CiccioSoft.Inventory.Infrastructure.Common;

namespace CiccioSoft.Inventory.Infrastructure
{
    public interface IAppSettings
    {
        string DbVersion { get; }
        bool IsRandomErrorsEnabled { get; set; }
        string UserName { get; set; }
        string Version { get; }
        string WindowsHelloPublicKeyHint { get; set; }
        DataProviderType DataProvider { get; set; }
        string SQLiteConnectionString { get; }
        string SQLServerConnectionString { get; set; }
        LogDatabaseType LogDatabase { get; set; }
        string LogMySqlConnectionString { get; set; }
    }
}
