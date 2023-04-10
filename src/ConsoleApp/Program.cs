using System;
using System.Linq;
using CommunityToolkit.Mvvm.DependencyInjection;
using Inventory.Infrastructure.Common;
using Inventory.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Inventory.Persistence;
using Microsoft.Extensions.Logging;

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
            //CreateEmptyDatabase();
            //ProvaDateTime();
            //FixCountryCodeTable();
        }

        private void CreateEmptyDatabase()
        {
            var optionsBuilder = new DbContextOptionsBuilder<SQLiteAppDbContext>();
            optionsBuilder.UseSqlite("Data Source=VanArsdelEmpty.db");
            //optionsBuilder.UseSqlite("Data Source=Database\\Empty.db");
            optionsBuilder.UseLoggerFactory(LoggerFactory.Create(b => b.AddConsole()));
            var db = new SQLiteAppDbContext(optionsBuilder.Options);
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
            var listCountry = db.CountryCodes.ToList();
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
            var optionsBuilder = new DbContextOptionsBuilder<SQLiteAppDbContext>();
            optionsBuilder.UseSqlite("Data Source=VanArsdel.db");
            optionsBuilder.UseLoggerFactory(LoggerFactory.Create(b => b.AddConsole()));
            var db = new SQLiteAppDbContext(optionsBuilder.Options);

            //var listCust = db.Customers.ToList();
            //foreach (var item in listCust)
            //{
            //    item.BirthDate2 = new DateTimeOffset((DateTime)item.BirthDate);
            //    item.LastModifiedOn2 = new DateTimeOffset((DateTime)item.LastModifiedOn);
            //    item.CreatedOn2 = new DateTimeOffset((DateTime)item.CreatedOn);
            //}
            //db.SaveChanges();

            //var listOrder = db.Orders.ToList();
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
    }
}
