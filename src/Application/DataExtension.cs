using CiccioSoft.Inventory.Application.Services;
using CiccioSoft.Inventory.Application.Services.Impl;
using CiccioSoft.Inventory.Infrastructure;
using CiccioSoft.Inventory.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace CiccioSoft.Inventory.Application
{
    public static class DataExtension
    {
        public static IServiceCollection AddInventoryData(this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddInventoryInfrastructure()
                .AddInventoryPersistence()
                .AddSingleton<ILogService, LogService>()
                .AddSingleton<ILookupTableService, LookupTableService>()
                .AddSingleton<ICustomerService, CustomerService>()
                .AddSingleton<IOrderService, OrderService>()
                .AddSingleton<IOrderItemService, OrderItemService>()
                .AddSingleton<IProductService, ProductService>();
            return serviceCollection;
        }
    }
}
