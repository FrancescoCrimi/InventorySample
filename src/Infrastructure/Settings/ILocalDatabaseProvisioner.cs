using Inventory.Infrastructure.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Inventory.Infrastructure.Settings
{
    public interface ILocalDatabaseProvisioner
    {
        /// <summary>
        /// Prepara il database principale (app.db) nella LocalFolder
        /// </summary>
        Task EnsureMainDatabaseAsync(CancellationToken ct = default);

        /// <summary>
        /// Prepara il database dei log (log.db) nella LocalFolder
        /// </summary>
        Task EnsureLogDatabaseAsync(CancellationToken ct = default);

        Task<Result> ResetLocalDatabaseAsync();

        string DatabaseLoggerConnectionString { get; }
    }
}
