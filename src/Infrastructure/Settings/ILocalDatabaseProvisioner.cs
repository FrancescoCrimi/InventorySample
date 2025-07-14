using System.Threading;
using System.Threading.Tasks;

namespace Inventory.Infrastructure.Settings
{
    public interface ILocalDatabaseProvisioner
    {
        /// <summary>
        /// Prepara il database principale (app.db) nella LocalFolder
        /// </summary>
        Task EnsureMainDatabaseAsync(/*DatabaseConfiguration config,*/ CancellationToken ct = default);

        /// <summary>
        /// Prepara il database dei log (log.db) nella LocalFolder
        /// </summary>
        Task EnsureLogDatabaseAsync(CancellationToken ct = default);

        ///// <summary>
        ///// Ritorna il path completo del database principale in LocalFolder\Database\{file}.
        ///// </summary>
        //string GetMainDatabasePath(DatabaseConfiguration config);

        ///// <summary>
        ///// Ritorna il path completo del database di log in LocalFolder\Database\log.db.
        ///// </summary>
        //string GetLogDatabasePath();

        string DatabaseLoggerConnectionString { get; }
    }
}
