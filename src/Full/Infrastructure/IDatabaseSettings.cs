using System;
using System.Threading;
using System.Threading.Tasks;

namespace Inventory.Infrastructure
{
    public interface IDatabaseSettings
    {
        Task CopyDataTables(Action<double> setValue,
                             Action<string> setStatus);
        void EnsureCreatedAsync(CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(CancellationToken cancellationToken = default);
        string GetDbVersion();
    }
}