using Inventory.Domain.Repository;
using Inventory.Infrastructure;
using Inventory.Persistence.DbContexts;
using Inventory.Persistence.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Persistence
{
    public static class InfrastructurePersistenceRegister
    {
        public static void Register(IServiceCollection serviceCollection)
        {
             serviceCollection
                .AddSingleton<IDatabaseMaintenanceService, DatabaseMaintenanceService>()
                .AddSingleton<AppDbContextFactory>()
                .AddTransient<AppDbContext>(provider =>
                    provider.GetService<AppDbContextFactory>().CreateDbContext())

                .AddTransient<ICustomerRepository, CustomerRepository>()
                .AddTransient<IOrderRepository, OrderRepository>()
                .AddTransient<IOrderItemRepository, OrderItemRepository>()
                .AddTransient<IProductRepository, ProductRepository>()
                .AddTransient<ILookupTableRepository, LookupTableRepository>()
                .AddSingleton<IDatabaseMaintenanceService, DatabaseMaintenanceService>();
        }
    }
}
