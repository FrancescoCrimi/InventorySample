using Inventory.Infrastructure;
using Inventory.Persistence;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Application
{
    public static class ApplicationExtension
    {
        public static IServiceCollection AddInventoryApplication(this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddInventoryInfrastructure()
                .AddInventoryPersistence()
                //.AddSingleton<ILogService, LogService>()
                //.AddSingleton<ILookupTableService, LookupTableService>()
                .AddSingleton<CustomerService>()
                .AddSingleton<OrderService>()
                .AddSingleton<OrderItemService>()
                .AddSingleton<ProductService>();
            return serviceCollection;
        }
    }
}
