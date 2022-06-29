using CiccioSoft.Inventory.Data.Services;
using CiccioSoft.Inventory.Domain;
using CiccioSoft.Inventory.Infrastructure;
using CiccioSoft.Inventory.Infrastructure.Common;
using CiccioSoft.Inventory.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CiccioSoft.Inventory.Persistence
{
    public static class InventoryPersistenceExtension
    {
        public static IServiceCollection AddInventoryPersistence(this IServiceCollection serviceCollection)
        {
            IAppSettings settings = serviceCollection.BuildServiceProvider().GetService<IAppSettings>();
            switch (settings.DataProvider)
            {
                case DataProviderType.SQLite:
                    serviceCollection.AddDbContext<SQLiteAppDbContext>(options =>
                    {
                        options
                            //.UseLazyLoadingProxies()
                            //.ConfigureWarnings(w => w.Ignore(CoreEventId.RowLimitingOperationWithoutOrderByWarning))
                            .UseSqlite(settings.SQLiteConnectionString);
                    }, ServiceLifetime.Transient, ServiceLifetime.Transient);
                    
                    serviceCollection.AddTransient<IAppDbContext>((serviceProvider)
                        => serviceProvider.GetRequiredService<SQLiteAppDbContext>());
                    
                    break;
                case DataProviderType.SQLServer:
                    serviceCollection.AddDbContext<SQLServerAppDbContext>(options =>
                    {
                        options
                            //.UseLazyLoadingProxies()
                            .UseSqlServer(settings.SQLServerConnectionString);
                    }, ServiceLifetime.Transient, ServiceLifetime.Transient);

                    serviceCollection.AddTransient<IAppDbContext>((serviceProvider)
                        => serviceProvider.GetRequiredService<SQLServerAppDbContext>());

                    break;
                case DataProviderType.WebAPI:
                    break;
                default:
                    break;
            }

            serviceCollection.AddTransient<IDataService, DataServiceBase>();

            return serviceCollection;
        }
    }
}
