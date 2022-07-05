using System;
using System.Threading;
using System.Threading.Tasks;

namespace CiccioSoft.Inventory.Persistence
{
    public interface IDatabaseSettings
    {
        Task CopyDataTables(Action<double> setValue,
                             Action<string> setStatus);
        Task<bool> EnsureCreatedAsync(CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(CancellationToken cancellationToken = default);
        string GetDbVersion();
    }
}