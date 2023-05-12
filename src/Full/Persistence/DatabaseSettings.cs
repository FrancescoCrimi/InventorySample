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
        private readonly AppDbContext _dbContext;
        private readonly DataProviderType _dataProviderType;
        private readonly IServiceProvider _serviceProvider;

        public DatabaseSettings(string connectionString,
                                DataProviderType dataProviderType,
                                IServiceProvider serviceProvider)
        {
            _dataProviderType = dataProviderType;
            _serviceProvider = serviceProvider;
            switch (dataProviderType)
            {
                case DataProviderType.SQLite:
                    var obSQLite = new DbContextOptionsBuilder<SQLiteAppDbContext>();
                    obSQLite.UseSqlite(connectionString);
                    _dbContext = new SQLiteAppDbContext(obSQLite.Options);
                    break;
                case DataProviderType.SQLServer:
                    var obSqlServer = new DbContextOptionsBuilder<SQLServerAppDbContext>();
                    obSqlServer.UseSqlServer(connectionString);
                    _dbContext = new SQLServerAppDbContext(obSqlServer.Options);
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
            var dbCreator = _dbContext.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
            return dbCreator.ExistsAsync(cancellationToken);
        }

        public string GetDbVersion()
        {
            var dbv = _dbContext.DbVersion.FirstOrDefault();
            return dbv?.Version;
        }

        public void EnsureCreatedAsync(CancellationToken cancellationToken = default)
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();
        }

        public async Task CopyDataTables(Action<double> setValue,
                                         Action<string> setStatus)
        {
            using (var sourceDb = _serviceProvider.GetService<AppDbContext>())
            {
                setStatus("Creating table Categories...");
                using (var trans = _dbContext.Database.BeginTransaction())
                {
                    foreach (var item in sourceDb.Categories.AsNoTracking())
                    {
                        await _dbContext.Categories.AddAsync(item);
                    }
                    if (_dataProviderType == DataProviderType.SQLServer)
                        _dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Categories ON;");
                    await _dbContext.SaveChangesAsync();
                    if (_dataProviderType == DataProviderType.SQLServer)
                        _dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Categories OFF;");
                    trans.Commit();
                }
                setValue(3);

                setStatus("Creating table CountryCodes...");
                using (var trans = _dbContext.Database.BeginTransaction())
                {
                    foreach (var item in sourceDb.Countries.AsNoTracking())
                    {
                        await _dbContext.Countries.AddAsync(item);
                    }
                    if (_dataProviderType == DataProviderType.SQLServer)
                        _dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Countries ON;");
                    await _dbContext.SaveChangesAsync();
                    if (_dataProviderType == DataProviderType.SQLServer)
                        _dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Countries OFF;");
                    trans.Commit();
                }
                setValue(4);

                setStatus("Creating table OrderStatus...");
                using (var trans = _dbContext.Database.BeginTransaction())
                {
                    foreach (var item in sourceDb.OrderStatuses.AsNoTracking())
                    {
                        await _dbContext.OrderStatuses.AddAsync(item);
                    }
                    if (_dataProviderType == DataProviderType.SQLServer)
                        _dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT OrderStatuses ON;");
                    await _dbContext.SaveChangesAsync();
                    if (_dataProviderType == DataProviderType.SQLServer)
                        _dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT OrderStatuses OFF;");
                    trans.Commit();
                }
                setValue(5);

                setStatus("Creating table PaymentTypes...");
                using (var trans = _dbContext.Database.BeginTransaction())
                {
                    foreach (var item in sourceDb.PaymentTypes.AsNoTracking())
                    {
                        await _dbContext.PaymentTypes.AddAsync(item);
                    }
                    if (_dataProviderType == DataProviderType.SQLServer)
                        _dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT PaymentTypes ON;");
                    await _dbContext.SaveChangesAsync();
                    if (_dataProviderType == DataProviderType.SQLServer)
                        _dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT PaymentTypes OFF;");
                    trans.Commit();
                }
                setValue(6);

                setStatus("Creating table Shippers...");
                using (var trans = _dbContext.Database.BeginTransaction())
                {
                    foreach (var item in sourceDb.Shippers.AsNoTracking())
                    {
                        await _dbContext.Shippers.AddAsync(item);
                    }
                    if (_dataProviderType == DataProviderType.SQLServer)
                        _dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Shippers ON;");
                    await _dbContext.SaveChangesAsync();
                    if (_dataProviderType == DataProviderType.SQLServer)
                        _dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Shippers OFF;");
                    trans.Commit();
                }
                setValue(7);

                setStatus("Creating table TaxTypes...");
                using (var trans = _dbContext.Database.BeginTransaction())
                {
                    foreach (var item in sourceDb.TaxTypes.AsNoTracking())
                    {
                        await _dbContext.TaxTypes.AddAsync(item);
                    }
                    if (_dataProviderType == DataProviderType.SQLServer)
                        _dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT TaxTypes ON;");
                    await _dbContext.SaveChangesAsync();
                    if (_dataProviderType == DataProviderType.SQLServer)
                        _dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT TaxTypes OFF;");
                    trans.Commit();
                }
                setValue(8);

                setStatus("Creating table Customers...");
                using (var trans = _dbContext.Database.BeginTransaction())
                {
                    foreach (var item in sourceDb.Customers.AsNoTracking())
                    {
                        await _dbContext.Customers.AddAsync(item);
                    }
                    if (_dataProviderType == DataProviderType.SQLServer)
                        _dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Customers ON;");
                    await _dbContext.SaveChangesAsync();
                    if (_dataProviderType == DataProviderType.SQLServer)
                        _dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Customers OFF;");
                    trans.Commit();
                }
                setValue(9);

                setStatus("Creating table Products...");
                using (var trans = _dbContext.Database.BeginTransaction())
                {
                    foreach (var item in sourceDb.Products.AsNoTracking())
                    {
                        await _dbContext.Products.AddAsync(item);
                    }
                    if (_dataProviderType == DataProviderType.SQLServer)
                        _dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Products ON;");
                    await _dbContext.SaveChangesAsync();
                    if (_dataProviderType == DataProviderType.SQLServer)
                        _dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Products OFF;");
                    trans.Commit();
                }
                setValue(10);

                setStatus("Creating table Orders...");
                using (var trans = _dbContext.Database.BeginTransaction())
                {
                    foreach (var item in sourceDb.Orders.AsNoTracking())
                    {
                        await _dbContext.Orders.AddAsync(item);
                    }
                    if (_dataProviderType == DataProviderType.SQLServer)
                        _dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Orders ON;");
                    await _dbContext.SaveChangesAsync();
                    if (_dataProviderType == DataProviderType.SQLServer)
                        _dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Orders OFF;");
                    trans.Commit();
                }
                setValue(11);

                setStatus("Creating table OrderItems...");
                using (var trans = _dbContext.Database.BeginTransaction())
                {
                    foreach (var item in sourceDb.OrderItems.AsNoTracking())
                    {
                        await _dbContext.OrderItems.AddAsync(item);
                    }
                    if (_dataProviderType == DataProviderType.SQLServer)
                        _dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT OrderItems ON;");
                    await _dbContext.SaveChangesAsync();
                    if (_dataProviderType == DataProviderType.SQLServer)
                        _dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT OrderItems OFF;");
                    trans.Commit();
                }
                setValue(12);

                setStatus("Creating database version...");
                await _dbContext.DbVersion.AddAsync(await sourceDb.DbVersion.FirstAsync());
                await _dbContext.SaveChangesAsync();
                setValue(13);
            }
        }

        public async Task CopyDatabase(Action<double> setValue,
                                         Action<string> setStatus)
        {
            //if (!await ExistsAsync())
            //{
            setValue(1);
            setStatus("Creating Database...");
            EnsureCreatedAsync();
            setValue(2);
            await CopyDataTables(setValue, setStatus);
            setValue(14);
            setStatus("Database created successfully.");
            //    Message = "Database created successfully.";
            //    Result = Result.Ok("Database created successfully.");
            //}
            //else
            //{
            //    ProgressValue = 14;
            //    Message = $"Database already exists. Please, delete database and try again.";
            //    Result = Result.Error("Database already exist");
            //}
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
                if (_dbContext != null)
                {
                    _dbContext.Dispose();
                }
            }
        }

        #endregion
    }
}
