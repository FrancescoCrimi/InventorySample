using Inventory.Domain.Repository;
using Inventory.Infrastructure;
using Inventory.Infrastructure.Settings;
using Inventory.Persistence.DbContexts;
using Inventory.Persistence.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Persistence
{
    public static class InfrastructurePersistenceRegister
    {
        public static void Register(IServiceCollection serviceCollection)
        {
            // Register your persistence services here
            // For example, you might register a database context or a repository
            // Example:
            // services.AddSingleton<IDatabaseContext, SqliteDatabaseContext>();
            // services.AddTransient<IRepository, RepositoryImplementation>();
            // You can also register other components if needed
            // services.AddSingleton<IUnitOfWork, UnitOfWorkImplementation>();

            var settings = serviceCollection.BuildServiceProvider().GetService<IAppSettings>();

            switch (settings.DataProvider)
            {
                case DatabaseProviderType.SQLite:
                    serviceCollection.AddDbContext<SQLiteAppDbContext>(options =>
                    {
                        options
                            //.UseLazyLoadingProxies()
                            .UseSqlite(settings.SQLiteConnectionString)
                            .EnableSensitiveDataLogging(true);
                    }, ServiceLifetime.Transient, ServiceLifetime.Transient);

                    serviceCollection.AddTransient<AppDbContext>((serviceProvider)
                        => serviceProvider.GetRequiredService<SQLiteAppDbContext>());
                    break;

                case DatabaseProviderType.SQLServer:
                    serviceCollection.AddDbContext<SQLServerAppDbContext>(options =>
                    {
                        options
                            //.UseLazyLoadingProxies()
                            .UseSqlServer(settings.SQLServerConnectionString);
                    }, ServiceLifetime.Transient, ServiceLifetime.Transient);

                    serviceCollection.AddTransient<AppDbContext>((serviceProvider)
                        => serviceProvider.GetRequiredService<SQLServerAppDbContext>());
                    break;

                case DatabaseProviderType.WebAPI:
                    break;

                default:
                    break;
            }

             serviceCollection
                .AddTransient<ICustomerRepository, CustomerRepository>()
                .AddTransient<IOrderRepository, OrderRepository>()
                .AddTransient<IOrderItemRepository, OrderItemRepository>()
                .AddTransient<IProductRepository, ProductRepository>()
                .AddTransient<ILookupTableRepository, LookupTableRepository>()
                .AddSingleton<IDatabaseMaintenanceService, DatabaseMaintenanceService>();
        }
    }
}
