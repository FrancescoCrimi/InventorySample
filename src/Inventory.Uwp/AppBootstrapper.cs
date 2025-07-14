using Inventory.Infrastructure.Settings;
using System.Threading;
using System.Threading.Tasks;

namespace Inventory.Uwp
{
    public class AppBootstrapper
    {
        //private readonly IServiceProvider _services;
        private readonly IDatabaseConfigurationService _databaseConfigurationService;
        private readonly ILocalDatabaseProvisioner _localDatabaseProvisioner;

        public AppBootstrapper(/*IServiceProvider services,*/ IDatabaseConfigurationService databaseConfigurationService , ILocalDatabaseProvisioner localDatabaseProvisioner)
        {
            //_services = services;
            _databaseConfigurationService = databaseConfigurationService;
            _localDatabaseProvisioner = localDatabaseProvisioner;
        }

        public async Task InitializeAsync(CancellationToken ct = default)
        {
            //var provisioner = _services.GetRequiredService<ILocalDatabaseProvisioner>();
            //var configService = _services.GetRequiredService<IDatabaseConfigurationService>();

            // 🧱 1. Prepara il DB principale se SQLite
            var current = _databaseConfigurationService.GetCurrent();
            if (current == null || current.Provider == DatabaseProviderType.SQLite &&
                current.Key == "DefaultSQLite")
            {
                await _localDatabaseProvisioner.EnsureMainDatabaseAsync(ct);
            }

            // 🗂️ 2. Prepara il DB dei log
            await _localDatabaseProvisioner.EnsureLogDatabaseAsync(ct);
        }
    }
}
