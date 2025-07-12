using System.Collections.Generic;

namespace Inventory.Infrastructure.Settings
{
    public interface IDatabaseConfigurationService
    {
        IReadOnlyList<DatabaseConfiguration> GetAll();
        DatabaseConfiguration GetCurrent();
        void SetCurrent(string key);
        void Add(DatabaseConfiguration config);
        void Remove(string key);
    }
}
