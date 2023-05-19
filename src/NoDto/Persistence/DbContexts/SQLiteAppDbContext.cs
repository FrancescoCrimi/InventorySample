// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.

using Inventory.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Inventory.Persistence.DbContexts
{
    public class SQLiteAppDbContext : AppDbContext
    {
        public SQLiteAppDbContext(DbContextOptions<SQLiteAppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>(t =>
            {
                t.Property(p => p.YearlyIncome).HasConversion<double>();
                t.Property(p => p.BirthDate).HasConversion(new DateTimeOffsetToBinaryConverter());
                t.Property(p => p.CreatedOn).HasConversion(new DateTimeOffsetToBinaryConverter());
                t.Property(p => p.LastModifiedOn).HasConversion(new DateTimeOffsetToBinaryConverter());
            });
            modelBuilder.Entity<OrderItem>(t =>
            {
                t.Property(p => p.UnitPrice).HasConversion<double>();
                t.Property(p => p.Discount).HasConversion<double>();
            });
            modelBuilder.Entity<Order>(t =>
            {
                t.Property(p => p.DeliveredDate).HasConversion(new DateTimeOffsetToBinaryConverter());
                t.Property(p => p.LastModifiedOn).HasConversion(new DateTimeOffsetToBinaryConverter());
                t.Property(p => p.OrderDate).HasConversion(new DateTimeOffsetToBinaryConverter());
                t.Property(p => p.ShippedDate).HasConversion(new DateTimeOffsetToBinaryConverter());
            });
            modelBuilder.Entity<Product>(t =>
            {
                t.Property(p => p.ListPrice).HasConversion<double>();
                t.Property(p => p.DealerPrice).HasConversion<double>();
                t.Property(p => p.Discount).HasConversion<double>();
                t.Property(p => p.CreatedOn).HasConversion(new DateTimeOffsetToBinaryConverter());
                t.Property(p => p.DiscountEndDate).HasConversion(new DateTimeOffsetToBinaryConverter());
                t.Property(p => p.DiscountStartDate).HasConversion(new DateTimeOffsetToBinaryConverter());
                t.Property(p => p.LastModifiedOn).HasConversion(new DateTimeOffsetToBinaryConverter());
            });
            modelBuilder.Entity<TaxType>(t =>
            {
                t.Property(p => p.Rate).HasConversion<double>();
            });

            // SQLite does not have proper support for DateTimeOffset via Entity Framework Core, see the limitations
            // here: https://docs.microsoft.com/en-us/ef/core/providers/sqlite/limitations#query-limitations
            // To work around this, when the Sqlite database provider is used, all model properties of type DateTimeOffset
            // use the DateTimeOffsetToBinaryConverter
            // Based on: https://github.com/aspnet/EntityFrameworkCore/issues/10784#issuecomment-415769754
            // This only supports millisecond precision, but should be sufficient for most use cases.
            //foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            //{
            //    var properties = entityType.ClrType.GetProperties().Where(p => p.PropertyType == typeof(DateTimeOffset)
            //                                                                || p.PropertyType == typeof(DateTimeOffset?));
            //    foreach (var property in properties)
            //    {
            //        modelBuilder
            //            .Entity(entityType.Name)
            //            .Property(property.Name)
            //            .HasConversion(new DateTimeOffsetToBinaryConverter());
            //    }

            //    var properties2 = entityType.ClrType.GetProperties().Where(p => p.PropertyType == typeof(decimal)
            //                                                                || p.PropertyType == typeof(decimal?));
            //    foreach (var property in properties2)
            //    {
            //        modelBuilder
            //            .Entity(entityType.Name)
            //            .Property(property.Name)
            //            .HasConversion<double>();
            //    }
            //}
        }
    }
}