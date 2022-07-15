using CiccioSoft.Inventory.Data.Services;
using CiccioSoft.Inventory.Domain.Repository;
using CiccioSoft.Inventory.Infrastructure;
using CiccioSoft.Inventory.Infrastructure.Common;
using CiccioSoft.Inventory.Persistence.DbContexts;
using CiccioSoft.Inventory.Persistence.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CiccioSoft.Inventory.Persistence
{
    public static class PersistenceExtension
    {
        public static IServiceCollection AddInventoryPersistence(this IServiceCollection serviceCollection)
        {
            IAppSettings settings = serviceCollection.BuildServiceProvider().GetService<IAppSettings>();
            serviceCollection
               .AddDbContext<LogDbContext>(option =>
                {
                    //option.UseSqlite(AppSettings.Current.LogConnectionString);
                    option.UseMySql(settings.LogMySqlConnectionString);
                    //option.UseSqlServer(AppSettings.Current.MsLogConnectionString);
                }, ServiceLifetime.Transient);

            serviceCollection.AddDbContext<SQLiteAppDbContext>(options =>
            {
                options
                    //.UseLazyLoadingProxies()
                    //.ConfigureWarnings(w => w.Ignore(CoreEventId.RowLimitingOperationWithoutOrderByWarning))
                    .UseSqlite(settings.SQLiteConnectionString);
            }, ServiceLifetime.Transient, ServiceLifetime.Transient);

            serviceCollection.AddDbContext<SQLServerAppDbContext>(options =>
            {
                options
                    //.UseLazyLoadingProxies()
                    .UseSqlServer(settings.SQLServerConnectionString);
            }, ServiceLifetime.Transient, ServiceLifetime.Transient);

            switch (settings.DataProvider)
            {
                case DataProviderType.SQLite:
                    serviceCollection.AddTransient<AppDbContext>((serviceProvider)
                        => serviceProvider.GetRequiredService<SQLiteAppDbContext>());
                    break;
                case DataProviderType.SQLServer:
                    serviceCollection.AddTransient<AppDbContext>((serviceProvider)
                        => serviceProvider.GetRequiredService<SQLServerAppDbContext>());
                    break;
                case DataProviderType.WebAPI:
                    break;
                default:
                    break;
            }

            serviceCollection
                .AddTransient<ILogRepository, LogRepository>()
                .AddTransient<ICustomerRepository, CustomerRepository>()
                .AddTransient<IOrderRepository, OrderRepository>()
                .AddTransient<IOrderItemRepository, OrderItemRepository>()
                .AddTransient<IProductRepository, ProductRepository>()
                .AddTransient<ILookupTableRepository, LookupTableRepository>();

            return serviceCollection;
        }
    }
}
