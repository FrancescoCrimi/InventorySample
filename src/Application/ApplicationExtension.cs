using CiccioSoft.Inventory.Application.Impl;
using CiccioSoft.Inventory.Infrastructure;
using CiccioSoft.Inventory.Persistence;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CiccioSoft.Inventory.Application
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
                .AddSingleton<ICustomerService, CustomerService>()
                .AddSingleton<IOrderService, OrderService>()
                .AddSingleton<IOrderItemService, OrderItemService>()
                .AddSingleton<IProductService, ProductService>();
            return serviceCollection;
        }
    }
}
