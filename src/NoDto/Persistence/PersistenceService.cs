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

using Inventory.Infrastructure;
using Inventory.Infrastructure.Common;
using Inventory.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inventory.Persistence
{
    public class PersistenceService : IPersistenceService
    {
        private readonly IServiceProvider _serviceProvider;

        public PersistenceService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<bool> ExistsAsync(string connectionString,
                                            DataProviderType dataProviderType,
                                            CancellationToken cancellationToken = default)
        {
            //var dbCreator = (RelationalDatabaseCreator)dbContext.Database.GetService<IDatabaseCreator>();
            using (var dbContext = GetAppDbContext(connectionString, dataProviderType))
            {
                var dbCreator = dbContext.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
                var rtn = await dbCreator.ExistsAsync(cancellationToken);
                return rtn;
            }
        }

        public string GetDbVersion(string connectionString,
                                   DataProviderType dataProviderType)
        {
            using (var dbContext = GetAppDbContext(connectionString, dataProviderType))
            {
                var dbv = dbContext.DbVersion.FirstOrDefault();
                var rtn = dbv?.Version;
                return rtn;
            }
        }

        public async Task<bool> EnsureCreatedAsync(string connectionString,
                                                   DataProviderType dataProviderType,
                                                   CancellationToken cancellationToken = default)
        {
            using (var dbContext = GetAppDbContext(connectionString, dataProviderType))
            {
                await dbContext.Database.EnsureDeletedAsync(cancellationToken);
                var rtn = await dbContext.Database.EnsureCreatedAsync(cancellationToken);
                return rtn;
            }
        }

        public async Task CopyDataTables(string connectionString,
                                         DataProviderType dataProviderType,
                                         Action<double> setValue,
                                         Action<string> setStatus)
        {
            using (var dbContext = GetAppDbContext(connectionString, dataProviderType))
            {
                using (var sourceDb = _serviceProvider.GetService<AppDbContext>())
                {
                    setStatus("Creating table Categories...");
                    foreach (var item in sourceDb.Categories.AsNoTracking())
                    {
                        await dbContext.Categories.AddAsync(item);
                    }
                    await dbContext.SaveChangesAsync();
                    setValue(3);

                    setStatus("Creating table CountryCodes...");
                    foreach (var item in sourceDb.Countries.AsNoTracking())
                    {
                        await dbContext.Countries.AddAsync(item);
                    }
                    await dbContext.SaveChangesAsync();
                    setValue(4);

                    setStatus("Creating table OrderStatus...");
                    foreach (var item in sourceDb.OrderStatuses.AsNoTracking())
                    {
                        await dbContext.OrderStatuses.AddAsync(item);
                    }
                    await dbContext.SaveChangesAsync();
                    setValue(5);

                    setStatus("Creating table PaymentTypes...");
                    foreach (var item in sourceDb.PaymentTypes.AsNoTracking())
                    {
                        await dbContext.PaymentTypes.AddAsync(item);
                    }
                    await dbContext.SaveChangesAsync();
                    setValue(6);

                    setStatus("Creating table Shippers...");
                    foreach (var item in sourceDb.Shippers.AsNoTracking())
                    {
                        await dbContext.Shippers.AddAsync(item);
                    }
                    await dbContext.SaveChangesAsync();
                    setValue(7);

                    setStatus("Creating table TaxTypes...");
                    foreach (var item in sourceDb.TaxTypes.AsNoTracking())
                    {
                        await dbContext.TaxTypes.AddAsync(item);
                    }
                    await dbContext.SaveChangesAsync();
                    setValue(8);

                    setStatus("Creating table Customers...");
                    foreach (var item in sourceDb.Customers.AsNoTracking())
                    {
                        await dbContext.Customers.AddAsync(item);
                    }
                    await dbContext.SaveChangesAsync();
                    setValue(9);

                    setStatus("Creating table Products...");
                    foreach (var item in sourceDb.Products.AsNoTracking())
                    {
                        await dbContext.Products.AddAsync(item);
                    }
                    await dbContext.SaveChangesAsync();
                    setValue(10);

                    setStatus("Creating table Orders...");
                    foreach (var item in sourceDb.Orders.AsNoTracking())
                    {
                        await dbContext.Orders.AddAsync(item);
                    }
                    await dbContext.SaveChangesAsync();
                    setValue(11);

                    setStatus("Creating table OrderItems...");
                    foreach (var item in sourceDb.OrderItems.AsNoTracking())
                    {
                        await dbContext.OrderItems.AddAsync(item);
                    }
                    await dbContext.SaveChangesAsync();
                    setValue(12);

                    setStatus("Creating database version...");
                    await dbContext.DbVersion.AddAsync(await sourceDb.DbVersion.FirstAsync());
                    await dbContext.SaveChangesAsync();
                    setValue(13);
                }
            }
        }

        private AppDbContext GetAppDbContext(string connectionString,
                                             DataProviderType dataProviderType)
        {
            switch (dataProviderType)
            {
                case DataProviderType.SQLite:
                    throw new NotImplementedException();
                case DataProviderType.SQLServer:
                    var optionsBuilder = new DbContextOptionsBuilder<SQLServerAppDbContext>();
                    optionsBuilder.UseSqlServer(connectionString);
                    var _dbContext = new SQLServerAppDbContext(optionsBuilder.Options);
                    return _dbContext;
                case DataProviderType.MySql:
                    throw new NotImplementedException();
                case DataProviderType.WebAPI:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
