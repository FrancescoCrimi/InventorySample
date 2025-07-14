using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Inventory.Infrastructure.Settings
{
    public class DatabaseConfigurationService : IDatabaseConfigurationService
    {
        private readonly ISettingsService _settings;
        private List<DatabaseConfiguration> _profiles;
        private string _currentKey;

        public DatabaseConfigurationService(ISettingsService settings)
        {
            _settings = settings;
            // carica dal JSON
            //_profiles = JsonSerializer.Deserialize<List<DatabaseConfiguration>>(
            //   _settings.Get(SettingKey.DatabaseProfiles) ?? "[]")!;
            _profiles = JsonSerializer.Deserialize<List<DatabaseConfiguration>>(
                _settings.Get(SettingKey.DatabaseProfiles) ?? "[]");
            _currentKey = _settings.Get(SettingKey.CurrentDatabaseKey)
                ?? "DefaultSQLite";
        }

        public IReadOnlyList<DatabaseConfiguration> GetAll() => _profiles;

        public DatabaseConfiguration GetCurrent()
        {
            return _profiles.FirstOrDefault(p => p.Key == _currentKey);
        }

        public void SetCurrent(string key)
        {
            if (!_profiles.Any(p => p.Key == key))
                throw new KeyNotFoundException(key);
            _currentKey = key;
            _settings.Set(SettingKey.CurrentDatabaseKey, key);
            _settings.Save();
        }

        public void Add(DatabaseConfiguration config)
        {
            if (_profiles.Any(p => p.Key == config.Key))
                throw new ArgumentException("Key già esistente");
            _profiles.Add(config);
            PersistProfiles();
        }

        public void Remove(string key)
        {
            var cfg = _profiles.FirstOrDefault(p => p.Key == key)
                      ?? throw new KeyNotFoundException(key);
            if (cfg.IsReadOnly)
                throw new InvalidOperationException("Non eliminabile");
            _profiles.Remove(cfg);
            if (_currentKey == key)
                SetCurrent("DefaultSQLite");
            PersistProfiles();
        }

        private void PersistProfiles()
        {
            var json = JsonSerializer.Serialize(_profiles);
            _settings.Set(SettingKey.DatabaseProfiles, json);
            _settings.Save();
        }
    }
}
