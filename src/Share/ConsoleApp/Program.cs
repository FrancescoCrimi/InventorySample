// Copyright (c) 2023 Francesco Crimi francrim@gmail.com
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.

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
            CreateEmptyDatabase();
            //FixCountryCodeTable();
            //FixOrderItemId();
        }

        private void CopyDatabase()
        {
            InitIoc();
            var connectionString = @"Data Source=.\SQLExpress;Initial Catalog=Inventory;Integrated Security=SSPI";
            //var connectionString = "Data Source = Database\\NewVanArsdel.1.01.db";
            var ps = Ioc.Default.GetService<PersistenceService>();
            ps.CopyDatabase(connectionString, DataProviderType.SQLServer, SetValue, SetStatus).Wait();
        }

        private void CreateEmptyDatabase()
        {
            var db = GetSqlServerAppDbContext();
            //var db = GetSQLiteAppDbContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            db.Dispose();
        }

        private void FixCountryCodeTable()
        {
            var db = GetSQLiteAppDbContext();
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
            InitIoc();
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


        private void InitIoc()
        {
            Ioc.Default.ConfigureServices(
                new ServiceCollection()
                .AddLogging(b => b.AddConsole())
                .AddSingleton<IAppSettings>(new AppSettings
                {
                    DataProvider = DataProviderType.SQLite,
                    SQLiteConnectionString = "Data Source = Database\\VanArsdel.1.01.db"
                })
                .AddInventoryInfrastructure()
                .AddInventoryPersistence()
                .BuildServiceProvider());
        }
 
        private AppDbContext GetSQLiteAppDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<SQLiteAppDbContext>()
                .UseLazyLoadingProxies()
                .UseSqlite("Data Source=VanArsdel.1.01.db")
                //.UseSqlite("Data Source=VanArsdelEmpty.db")
                //.UseSqlite("Data Source=Database\\Empty.db")
                .UseLoggerFactory(LoggerFactory.Create(b => b.AddConsole()));
            var dbContext = new SQLiteAppDbContext(optionsBuilder.Options);
            return dbContext;
        }

        private AppDbContext GetSqlServerAppDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<SQLServerAppDbContext>()
                .UseLazyLoadingProxies()
                //.UseSqlServer(@"Data Source=.\SQLExpress;Initial Catalog=Inventory;Integrated Security=SSPI")
                .UseSqlServer(@"Data Source=LAPTOP-VKK31C74\SQLEXPRESS;Initial Catalog=Inventory;Integrated Security=True;Persist Security Info=False")
                .UseLoggerFactory(LoggerFactory.Create(b => b.AddConsole()));
            var db = new SQLServerAppDbContext(optionsBuilder.Options);
            return db;
        }

        private void SetValue(double obj)
        {
            Console.WriteLine(obj.ToString());
        }

        private void SetStatus(string obj)
        {
            Console.WriteLine($"{obj}");
        }
    }
}
