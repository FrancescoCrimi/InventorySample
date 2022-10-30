using System;
using Inventory.Infrastructure.Common;

namespace Inventory.Infrastructure
{
    public interface IAppSettings
    {
        string DbVersion
        {
            get;
        }
        bool IsRandomErrorsEnabled
        {
            get; set;
        }
        string UserName
        {
            get; set;
        }
        string Version
        {
            get;
        }
        string WindowsHelloPublicKeyHint
        {
            get; set;
        }

        DataProviderType DataProvider
        {
            get; set;
        }
        string SQLiteConnectionString
        {
            get;
        }
        string SQLServerConnectionString
        {
            get; set;
        }
        string LogConnectionString
        {
            get;
        }
        string AppLogConnectionString
        {
            get;
        }
    }
}
