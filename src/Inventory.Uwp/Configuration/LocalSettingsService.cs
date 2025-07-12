using Inventory.Infrastructure.Settings;
using Windows.Storage;

namespace Inventory.Uwp.Configuration
{
    public class LocalSettingsService : ISettingsService
    {
        private readonly ApplicationDataContainer _settings =
            ApplicationData.Current.LocalSettings;

        //public string? Get(SettingKey key) =>
        public string Get(SettingKey key) =>
            _settings.Values[key.ToString()] as string;

        public void Set(SettingKey key, string value) =>
            _settings.Values[key.ToString()] = value;

        public void Save() { /* LocalSettings salva automaticamente */ }
    }
}
