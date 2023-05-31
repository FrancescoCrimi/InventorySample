// Copyright (c) Microsoft. All rights reserved.
// Copyright (c) 2023 Francesco Crimi francrim@gmail.com
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.

using Inventory.Domain.Aggregates.CustomerAggregate;
using Inventory.Domain.Aggregates.OrderAggregate;
using Inventory.Domain.Aggregates.ProductAggregate;
using Inventory.Domain.Repository;
using Inventory.Infrastructure;
using Inventory.Infrastructure.Common;
using Inventory.Persistence.DbContexts;
using Inventory.Persistence.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Persistence
{
    public static class PersistenceExtension
    {
        public static IServiceCollection AddInventoryPersistence(this IServiceCollection serviceCollection)
        {
            var settings = serviceCollection.BuildServiceProvider().GetService<IAppSettings>();

            serviceCollection.AddDbContext<SQLiteAppDbContext>(options =>
            {
                options
                    .UseLazyLoadingProxies()
                    //.EnableSensitiveDataLogging(true)
                    .UseSqlite(settings.SQLiteConnectionString);
                //}, ServiceLifetime.Transient, ServiceLifetime.Transient);
            });

            serviceCollection.AddDbContext<SQLServerAppDbContext>(options =>
            {
                options
                    .UseLazyLoadingProxies()
                    //.EnableSensitiveDataLogging(true)
                    .UseSqlServer(settings.SQLServerConnectionString);
                //}, ServiceLifetime.Transient, ServiceLifetime.Transient);
            });

            switch (settings.DataProvider)
            {
                case DataProviderType.SQLite:
                    serviceCollection.AddTransient<AppDbContext>((serviceProvider)
                        => serviceProvider.GetRequiredService<SQLiteAppDbContext>());
                    break;
                case DataProviderType.SQLServer:
                    serviceCollection.AddTransient<AppDbContext>((serviceProvider)
                        => serviceProvider.GetRequiredService<SQLServerAppDbContext>());
                    break;
                case DataProviderType.WebAPI:
                    break;
                default:
                    break;
            }

            return serviceCollection
                .AddTransient<ICustomerRepository, CustomerRepository>()
                .AddTransient<IOrderRepository, OrderRepository>()
                .AddTransient<IOrderItemRepository, OrderItemRepository>()
                .AddTransient<IProductRepository, ProductRepository>()
                .AddSingleton<IPersistenceService, PersistenceService>();
        }
    }
}
