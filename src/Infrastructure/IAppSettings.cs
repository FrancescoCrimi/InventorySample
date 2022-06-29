using System;

namespace CiccioSoft.Inventory.Infrastructure
{
    public interface IAppSettings
    {
        DataProviderType DataProvider { get; set; }
        string DbVersion { get; }
        bool IsRandomErrorsEnabled { get; set; }
        string SQLServerConnectionString { get; set; }
        string UserName { get; set; }
        string Version { get; }
        string WindowsHelloPublicKeyHint { get; set; }
        string SQLiteConnectionString { get; }
    }
}
