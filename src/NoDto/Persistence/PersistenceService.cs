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

        public async Task CopyDatabase(string connectionString,
                                       DataProviderType dataProviderType,
                                       Action<double> setValue,
                                       Action<string> setStatus,
                                       CancellationToken cancellationToken = default)
        {
            using (var dbContext = GetAppDbContext(connectionString, dataProviderType))
            {
                using (var sourceDb = _serviceProvider.GetService<AppDbContext>())
                {
                    setStatus("Deleting Database...");
                    await dbContext.Database.EnsureDeletedAsync(cancellationToken);
                    setValue(1);

                    setStatus("Creating Database...");
                    await dbContext.Database.EnsureCreatedAsync(cancellationToken);
                    setValue(2);

                    setStatus("Creating table Categories...");
                    using (var trans = dbContext.Database.BeginTransaction())
                    {
                        foreach (var item in sourceDb.Categories.AsNoTracking())
                        {
                            await dbContext.Categories.AddAsync(item);
                        }
                        if (dataProviderType == DataProviderType.SQLServer)
                            dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Categories ON;");
                        await dbContext.SaveChangesAsync();
                        if (dataProviderType == DataProviderType.SQLServer)
                            dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Categories OFF;");
                        trans.Commit();
                    }
                    setValue(3);


                    setStatus("Creating table Countries...");
                    using (var trans = dbContext.Database.BeginTransaction())
                    {
                        foreach (var item in sourceDb.Countries.AsNoTracking())
                        {
                            await dbContext.Countries.AddAsync(item);
                        }
                        if (dataProviderType == DataProviderType.SQLServer)
                            dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Countries ON;");
                        await dbContext.SaveChangesAsync();
                        if (dataProviderType == DataProviderType.SQLServer)
                            dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Countries OFF;");
                        trans.Commit();
                    }
                    setValue(4);

                    setStatus("Creating table OrderStatus...");
                    using (var trans = dbContext.Database.BeginTransaction())
                    {
                        foreach (var item in sourceDb.OrderStatuses.AsNoTracking())
                        {
                            await dbContext.OrderStatuses.AddAsync(item);
                        }
                        if (dataProviderType == DataProviderType.SQLServer)
                            dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT OrderStatuses ON;");
                        await dbContext.SaveChangesAsync();
                        if (dataProviderType == DataProviderType.SQLServer)
                            dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT OrderStatuses OFF;");
                        trans.Commit();
                    }
                    setValue(5);

                    setStatus("Creating table PaymentTypes...");
                    using (var trans = dbContext.Database.BeginTransaction())
                    {
                        foreach (var item in sourceDb.PaymentTypes.AsNoTracking())
                        {
                            await dbContext.PaymentTypes.AddAsync(item);
                        }
                        if (dataProviderType == DataProviderType.SQLServer)
                            dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT PaymentTypes ON;");
                        await dbContext.SaveChangesAsync();
                        if (dataProviderType == DataProviderType.SQLServer)
                            dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT PaymentTypes OFF;");
                        trans.Commit();
                    }
                    setValue(6);

                    setStatus("Creating table Shippers...");
                    using (var trans = dbContext.Database.BeginTransaction())
                    {
                        foreach (var item in sourceDb.Shippers.AsNoTracking())
                        {
                            await dbContext.Shippers.AddAsync(item);
                        }
                        if (dataProviderType == DataProviderType.SQLServer)
                            dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Shippers ON;");
                        await dbContext.SaveChangesAsync();
                        if (dataProviderType == DataProviderType.SQLServer)
                            dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Shippers OFF;");
                        trans.Commit();
                    }
                    setValue(7);

                    setStatus("Creating table TaxTypes...");
                    using (var trans = dbContext.Database.BeginTransaction())
                    {
                        foreach (var item in sourceDb.TaxTypes.AsNoTracking())
                        {
                            await dbContext.TaxTypes.AddAsync(item);
                        }
                        if (dataProviderType == DataProviderType.SQLServer)
                            dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT TaxTypes ON;");
                        await dbContext.SaveChangesAsync();
                        if (dataProviderType == DataProviderType.SQLServer)
                            dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT TaxTypes OFF;");
                        trans.Commit();
                    }
                    setValue(8);

                    setStatus("Creating table Customers...");
                    using (var trans = dbContext.Database.BeginTransaction())
                    {
                        foreach (var item in sourceDb.Customers.AsNoTracking())
                        {
                            await dbContext.Customers.AddAsync(item);
                        }
                        if (dataProviderType == DataProviderType.SQLServer)
                            dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Customers ON;");
                        await dbContext.SaveChangesAsync();
                        if (dataProviderType == DataProviderType.SQLServer)
                            dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Customers OFF;");
                        trans.Commit();
                    }
                    setValue(9);

                    setStatus("Creating table Products...");
                    using (var trans = dbContext.Database.BeginTransaction())
                    {
                        foreach (var item in sourceDb.Products.AsNoTracking())
                        {
                            await dbContext.Products.AddAsync(item);
                        }
                        if (dataProviderType == DataProviderType.SQLServer)
                            dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Products ON;");
                        await dbContext.SaveChangesAsync();
                        if (dataProviderType == DataProviderType.SQLServer)
                            dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Products OFF;");
                        trans.Commit();
                    }
                    setValue(10);

                    setStatus("Creating table Orders...");
                    using (var trans = dbContext.Database.BeginTransaction())
                    {
                        foreach (var item in sourceDb.Orders.AsNoTracking())
                        {
                            await dbContext.Orders.AddAsync(item);
                        }
                        if (dataProviderType == DataProviderType.SQLServer)
                            dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Orders ON;");
                        await dbContext.SaveChangesAsync();
                        if (dataProviderType == DataProviderType.SQLServer)
                            dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Orders OFF;");
                        trans.Commit();
                    }
                    setValue(11);

                    setStatus("Creating table OrderItems...");
                    using (var trans = dbContext.Database.BeginTransaction())
                    {
                        foreach (var item in sourceDb.OrderItems.AsNoTracking())
                        {
                            await dbContext.OrderItems.AddAsync(item);
                        }
                        if (dataProviderType == DataProviderType.SQLServer)
                            dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT OrderItems ON;");
                        await dbContext.SaveChangesAsync();
                        if (dataProviderType == DataProviderType.SQLServer)
                            dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT OrderItems OFF;");
                        trans.Commit();
                    }
                    setValue(12);

                    setStatus("Creating database version...");
                    await dbContext.DbVersion.AddAsync(await sourceDb.DbVersion.FirstAsync());
                    await dbContext.SaveChangesAsync();
                    setValue(13);

                    setValue(14);
                    setStatus("Database created successfully.");
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
