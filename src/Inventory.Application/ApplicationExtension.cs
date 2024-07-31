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
            return serviceCollection
                .AddInventoryInfrastructure()
                .AddInventoryPersistence()
                .AddSingleton<LookupTablesService>()
                .AddSingleton<ProductService>()
                .AddSingleton<CustomerService>()
                .AddSingleton<OrderService>()
                .AddSingleton<OrderItemService>();
        }
    }
}
