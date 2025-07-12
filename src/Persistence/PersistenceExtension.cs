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

using Inventory.Domain.Repository;
using Inventory.Infrastructure;
using Inventory.Infrastructure.Settings;
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

            switch (settings.DataProvider)
            {
                case DatabaseProviderType.SQLite:
                    serviceCollection.AddDbContext<SQLiteAppDbContext>(options =>
                    {
                        options
                            //.UseLazyLoadingProxies()
                            .UseSqlite(settings.SQLiteConnectionString)
                            .EnableSensitiveDataLogging(true);
                    }, ServiceLifetime.Transient, ServiceLifetime.Transient);

                    serviceCollection.AddTransient<AppDbContext>((serviceProvider)
                        => serviceProvider.GetRequiredService<SQLiteAppDbContext>());
                    break;

                case DatabaseProviderType.SQLServer:
                    serviceCollection.AddDbContext<SQLServerAppDbContext>(options =>
                    {
                        options
                            //.UseLazyLoadingProxies()
                            .UseSqlServer(settings.SQLServerConnectionString);
                    }, ServiceLifetime.Transient, ServiceLifetime.Transient);

                    serviceCollection.AddTransient<AppDbContext>((serviceProvider)
                        => serviceProvider.GetRequiredService<SQLServerAppDbContext>());
                    break;

                case DatabaseProviderType.WebAPI:
                    break;

                default:
                    break;
            }

            return serviceCollection
                .AddTransient<ICustomerRepository, CustomerRepository>()
                .AddTransient<IOrderRepository, OrderRepository>()
                .AddTransient<IOrderItemRepository, OrderItemRepository>()
                .AddTransient<IProductRepository, ProductRepository>()
                .AddTransient<ILookupTableRepository, LookupTableRepository>()
                .AddSingleton<IDatabaseMaintenanceService, DatabaseMaintenanceService>();
        }
    }
}
