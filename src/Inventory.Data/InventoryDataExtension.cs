using CiccioSoft.Inventory.Data.DataServices;
using CiccioSoft.Inventory.Data.Services;
using CiccioSoft.Inventory.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace CiccioSoft.Inventory.Data
{
    public static class InventoryDataExtension
    {
        public static IServiceCollection AddInventoryData(this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddInventoryPersistence()
                .AddTransient<ILogDataService, LogDataService>()
                .AddSingleton<ILookupTables, LookupTables>()
                .AddSingleton<ICustomerService, CustomerService>()
                .AddSingleton<IOrderService, OrderService>()
                .AddSingleton<IOrderItemService, OrderItemService>()
                .AddSingleton<IProductService, ProductService>();
            return serviceCollection;
        }
    }
}
