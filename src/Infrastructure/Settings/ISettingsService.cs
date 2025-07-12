using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Infrastructure.Settings
{
    public interface ISettingsService
    {
        //string? Get(SettingKey key);
        string Get(SettingKey key);
        void Set(SettingKey key, string value);
        void Save();
    }
}
