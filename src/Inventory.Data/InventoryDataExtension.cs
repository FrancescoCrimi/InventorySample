using CiccioSoft.Inventory.Data.Services;
using CiccioSoft.Inventory.Data.Services.Impl;
using CiccioSoft.Inventory.Infrastructure;
using CiccioSoft.Inventory.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace CiccioSoft.Inventory.Data
{
    public static class InventoryDataExtension
    {
        public static IServiceCollection AddInventoryData(this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddInventoryInfrastructure()
                .AddInventoryPersistence()
                .AddSingleton<ILookupTableService, LookupTableService>()
                .AddSingleton<ICustomerService, CustomerService>()
                .AddSingleton<IOrderService, OrderService>()
                .AddSingleton<IOrderItemService, OrderItemService>()
                .AddSingleton<IProductService, ProductService>();
            return serviceCollection;
        }
    }
}
