using CiccioSoft.Inventory.Data.DataServices;
using CiccioSoft.Inventory.Data.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CiccioSoft.Inventory.Data
{
    public static class InventoryCoreExtension
    {
        public static IServiceCollection AddInventoryCore(this IServiceCollection serviceCollection)
        {
            serviceCollection
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
