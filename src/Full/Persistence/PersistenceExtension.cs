using Inventory.Domain.Repository;
using Inventory.Infrastructure;
using Inventory.Infrastructure.Common;
using Inventory.Persistence.DbContexts;
using Inventory.Persistence.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Persistence
{
    public static class PersistenceExtension
    {
        public static IServiceCollection AddInventoryPersistence(this IServiceCollection serviceCollection)
        {
            var settings = serviceCollection.BuildServiceProvider().GetService<IAppSettings>();

            serviceCollection.AddDbContext<SQLiteAppDbContext>(options =>
            {
                options
                    //.UseLazyLoadingProxies()
                    .UseSqlite(settings.SQLiteConnectionString);
                    //.EnableSensitiveDataLogging(true);
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
                .AddTransient<ICustomerRepository, CustomerRepository>()
                .AddTransient<IOrderRepository, OrderRepository>()
                .AddTransient<IOrderItemRepository, OrderItemRepository>()
                .AddTransient<IProductRepository, ProductRepository>()
                .AddTransient<ILookupTableRepository, LookupTableRepository>();

            return serviceCollection;
        }
    }
}
