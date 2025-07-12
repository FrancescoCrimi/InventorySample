using Inventory.Infrastructure.Settings;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inventory.Uwp
{
    public class AppBootstrapper
    {
        private readonly IServiceProvider _services;

        public AppBootstrapper(IServiceProvider services)
        {
            _services = services;
        }

        public async Task InitializeAsync(CancellationToken ct = default)
        {
            var provisioner = _services.GetRequiredService<ILocalDatabaseProvisioner>();
            var configService = _services.GetRequiredService<IDatabaseConfigurationService>();

            // 🧱 1. Prepara il DB principale se SQLite
            var current = configService.GetCurrent();
            if (current.Provider == DatabaseProviderType.SQLite &&
                current.Key == "DefaultSQLite")
            {
                await provisioner.EnsureMainDatabaseAsync(current, ct);
            }

            // 🗂️ 2. Prepara il DB dei log
            await provisioner.EnsureLogDatabaseAsync(ct);

            // 🔒 3. Registra profilo SQLite se non esiste
            if (!configService.GetAll().Any(c => c.Key == "DefaultSQLite"))
            {
                configService.Add(new DatabaseConfiguration
                (
                    key: "DefaultSQLite",
                    provider: DatabaseProviderType.SQLite,
                    cs: "Data Source=app.db;",
                    isReadOnly: true
                ));
                configService.SetCurrent("DefaultSQLite");
            }
        }
    }
}
