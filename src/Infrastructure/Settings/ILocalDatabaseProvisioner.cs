using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inventory.Infrastructure.Settings
{
    public interface ILocalDatabaseProvisioner
    {
        /// Prepara il database principale (app.db) nella LocalFolder
        Task EnsureMainDatabaseAsync(DatabaseConfiguration config, CancellationToken ct = default);

        /// Prepara il database dei log (log.db) nella LocalFolder
        Task EnsureLogDatabaseAsync(CancellationToken ct = default);
    }
}
