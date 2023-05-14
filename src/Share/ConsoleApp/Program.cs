using CommunityToolkit.Mvvm.DependencyInjection;
using Inventory.Domain.Repository;
using Inventory.Infrastructure;
using Inventory.Infrastructure.Common;
using Inventory.Persistence;
using Inventory.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            new Program();
        }
        public Program()
        {
            //CopyDatabase();
            //CreateEmptyDatabase();
            //ProvaDateTime();
            //FixCountryCodeTable();
            TestGetOrders();
        }

        private void initIoc()
        {
            Ioc.Default.ConfigureServices(
                new ServiceCollection()
                .AddLogging(b => b.AddConsole())
                .AddSingleton<IAppSettings>(new AppSettings
                {
                    DataProvider = DataProviderType.SQLite,
                    //SQLiteConnectionString = "Data Source = Database\\VanArsdel.1.01.db"
                    SQLiteConnectionString = "Data Source = Database\\NewVanArsdel.1.01.db"
                })
                .AddInventoryInfrastructure()
                .AddInventoryPersistence()
                .BuildServiceProvider());
        }

        private void CopyDatabase()
        {
            initIoc();
            //var connectionString = @"Data Source=.\SQLExpress;Initial Catalog=Inventory;Integrated Security=SSPI";
            var cnSQLite = "Data Source = Database\\NewVanArsdel.1.01.db";
            using (var db = new DatabaseSettings(cnSQLite, DataProviderType.SQLite, Ioc.Default))
            {
                //db.CopyDatabase(SetValue, SetStatus).Wait();
                //db.EnsureCreatedAsync();
            }
        }

        private void SetValue(double obj)
        {
            Console.WriteLine(obj.ToString());
        }

        private void SetStatus(string obj)
        {
            Console.WriteLine($"{obj}");
        }

        private SQLServerAppDbContext GetSqlServer()
        {
            var optionsBuilder = new DbContextOptionsBuilder<SQLServerAppDbContext>();
            optionsBuilder.UseSqlServer(@"Data Source=.\SQLExpress;Initial Catalog=Inventory;Integrated Security=SSPI");
            optionsBuilder.UseLoggerFactory(LoggerFactory.Create(b => b.AddConsole()));
            var db = new SQLServerAppDbContext(optionsBuilder.Options);
            return db;
        }

        private SQLiteAppDbContext GetSQLite()
        {
            var optionsBuilder = new DbContextOptionsBuilder<SQLiteAppDbContext>();
            optionsBuilder.UseSqlite("Data Source=VanArsdelEmpty.db");
            //optionsBuilder.UseSqlite("Data Source=Database\\Empty.db");
            optionsBuilder.UseLoggerFactory(LoggerFactory.Create(b => b.AddConsole()));
            var db = new SQLiteAppDbContext(optionsBuilder.Options);
            return db;
        }

        private void CreateEmptyDatabase()
        {
            var db = GetSqlServer();
            //var db = GetSQLite();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            db.Dispose();
        }

        private void FixCountryCodeTable()
        {
            var optionsBuilder = new DbContextOptionsBuilder<SQLiteAppDbContext>();
            optionsBuilder.UseSqlite("Data Source=VanArsdel.1.01.db");
            optionsBuilder.UseLoggerFactory(LoggerFactory.Create(b => b.AddConsole()));
            var db = new SQLiteAppDbContext(optionsBuilder.Options);
            var listCountry = db.Countries.ToList();
            for (var i = 0; i < listCountry.Count; i++)
            {
                var country = listCountry[i];
                country.Id = i + 1;
            }
            db.SaveChanges();
            db.Dispose();
        }

        private void FixOrderItemId()
        {
            var db = Ioc.Default.GetRequiredService<AppDbContext>();
            var list = db.OrderItems.ToList();
            foreach (var item in list)
            {
                item.Id = UIDGenerator.Next();
                System.Threading.Thread.Sleep(10);
            }
            db.SaveChanges();
            db.Dispose();
        }

        private void ProvaDateTime()
        {
            var optionsBuilder = new DbContextOptionsBuilder<SQLiteAppDbContext>()
                .UseLazyLoadingProxies()
                .UseSqlite("Data Source=VanArsdel.1.01.db")
                .UseLoggerFactory(LoggerFactory.Create(b => b.AddConsole()));
            var db = new SQLiteAppDbContext(optionsBuilder.Options);

            //var listCust = db.Customers.ToList();
            //foreach (var item in listCust)
            //{
            //    item.BirthDate2 = new DateTimeOffset((DateTime)item.BirthDate);
            //    item.LastModifiedOn2 = new DateTimeOffset((DateTime)item.LastModifiedOn);
            //    item.CreatedOn2 = new DateTimeOffset((DateTime)item.CreatedOn);
            //}
            //db.SaveChanges();

            var listOrder = db.Orders.ToList();
            //foreach (var item in listOrder)
            //{
            //    if (item.DeliveredDate == null) item.DeliveredDate2 = null;
            //    else item.DeliveredDate2 = new DateTimeOffset((DateTime)item.DeliveredDate);

            //    item.LastModifiedOn2 = new DateTimeOffset((DateTime)item.LastModifiedOn);
            //    item.OrderDate2 = new DateTimeOffset((DateTime)item.OrderDate);

            //    if (item.ShippedDate == null) item.ShippedDate2 = null;
            //    else item.ShippedDate2 = new DateTimeOffset((DateTime)item.ShippedDate);
            //}
            //db.SaveChanges();

            //var listProd = db.Products.ToList();
            //foreach (var item in listProd)
            //{
            //    item.CreatedOn2 = new DateTimeOffset((DateTime)item.CreatedOn);

            //    if (item.DiscountEndDate == null) item.DiscountEndDate2 = null;
            //    else item.DiscountEndDate2 = new DateTimeOffset((DateTime)item.DiscountEndDate);

            //    if(item.DiscountStartDate == null) item.DiscountStartDate2 = null;
            //    else item.DiscountStartDate2 = new DateTimeOffset((DateTime)item.DiscountStartDate);

            //    item.LastModifiedOn2 = new DateTimeOffset((DateTime)item.LastModifiedOn);
            //}
            //db.SaveChanges();

            db.Dispose();
        }

        private void TestGetOrders()
        {
            initIoc();
            IOrderRepository repo = Ioc.Default.GetService<IOrderRepository>();
            var orders = repo.GetOrdersAsync(0, 100, new DataRequest<Inventory.Domain.Model.Order>()).Result;
        }
    }
}
