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
    public class DatabaseSettings : IDisposable, IDatabaseSettings
    {
        private readonly AppDbContext dbContext;
        private readonly IServiceProvider serviceProvider;

        public DatabaseSettings(string connectionString,
                                DataProviderType dataProviderType,
                                IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            switch (dataProviderType)
            {
                case DataProviderType.SQLite:
                    throw new NotImplementedException();
                case DataProviderType.SQLServer:
                    var optionsBuilder = new DbContextOptionsBuilder<SQLServerAppDbContext>();
                    optionsBuilder.UseSqlServer(connectionString);
                    dbContext = new SQLServerAppDbContext(optionsBuilder.Options);
                    break;
                case DataProviderType.MySql:
                    throw new NotImplementedException();
                case DataProviderType.WebAPI:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }
        }

        public Task<bool> ExistsAsync(CancellationToken cancellationToken = default)
        {
            var dbCreator = dbContext.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
            return dbCreator.ExistsAsync(cancellationToken);
        }

        public string GetDbVersion()
        {
            var dbv = dbContext.DbVersion.FirstOrDefault();
            return dbv != null ? dbv.Version : null;
        }

        public Task<bool> EnsureCreatedAsync(CancellationToken cancellationToken = default)
        {
            return dbContext.Database.EnsureCreatedAsync(cancellationToken);
        }

        public async Task CopyDataTables(Action<double> setValue,
                                         Action<string> setStatus)
        {
            using (var sourceDb = serviceProvider.GetService<AppDbContext>())
            {
                setStatus("Creating table Categories...");
                foreach (var item in sourceDb.Categories.AsNoTracking())
                {
                    await dbContext.Categories.AddAsync(item);
                }
                await dbContext.SaveChangesAsync();
                setValue(3);

                setStatus("Creating table CountryCodes...");
                foreach (var item in sourceDb.CountryCodes.AsNoTracking())
                {
                    await dbContext.CountryCodes.AddAsync(item);
                }
                await dbContext.SaveChangesAsync();
                setValue(4);

                setStatus("Creating table OrderStatus...");
                foreach (var item in sourceDb.OrderStatus.AsNoTracking())
                {
                    await dbContext.OrderStatus.AddAsync(item);
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

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        #endregion
    }
}
