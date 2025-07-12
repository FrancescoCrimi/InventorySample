using Inventory.Infrastructure.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Inventory.Uwp.Configuration
{
    public class JsonSettingsService : ISettingsService
    {
        private readonly string _path =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                         "MyApp", "settings.json");
        private readonly Dictionary<SettingKey, string> _cache;

        public JsonSettingsService()
        {
            if (File.Exists(_path))
                _cache = JsonSerializer.Deserialize<Dictionary<SettingKey, string>>(File.ReadAllText(_path))
                         ?? new Dictionary<SettingKey, string>();
            else
                _cache = new Dictionary<SettingKey, string>();
        }

        //public string? Get(SettingKey key) =>
        public string Get(SettingKey key) =>
            _cache.TryGetValue(key, out var v) ? v : null;

        public void Set(SettingKey key, string value) =>
            _cache[key] = value;

        public void Save()
        {
            //Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
            Directory.CreateDirectory(Path.GetDirectoryName(_path));
            File.WriteAllText(_path, JsonSerializer.Serialize(_cache, new JsonSerializerOptions
            {
                WriteIndented = true
            }));
        }
    }
}
