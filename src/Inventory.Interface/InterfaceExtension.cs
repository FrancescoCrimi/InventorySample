using Inventory.Application;
using Inventory.Interface.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Inventory.Interface
{
    public static class InterfaceExtension
    {
        public static IServiceCollection AddInventoryInterface(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddInventoryApplication()
                .AddSingleton<LookupTablesServiceFacade>()
                .AddSingleton<ProductServiceFacade>()
                .AddSingleton<CustomerServiceFacade>()
                .AddSingleton<OrderServiceFacade>()
                .AddSingleton<OrderItemServiceFacade>();
        }
    }
}
