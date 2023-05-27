// Copyright (c) Microsoft. All rights reserved.
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
using Microsoft.EntityFrameworkCore;

namespace Inventory.Persistence.DbContexts
{
    public abstract class AppDbContext : DbContext
    {
        protected AppDbContext() { }

        protected AppDbContext(DbContextOptions options)
            : base(options) { }

        public DbSet<DbVersion> DbVersion { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<PaymentType> PaymentTypes { get; set; }
        public DbSet<Shipper> Shippers { get; set; }
        public DbSet<TaxType> TaxTypes { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            CategoryMapping(modelBuilder);
            CountryMapping(modelBuilder);
            CustomerMapping(modelBuilder);
            OrderItemMapping(modelBuilder);
            OrderMapping(modelBuilder);
            OrderStatusMapping(modelBuilder);
            PaymentTypeMapping(modelBuilder);
            ProductMapping(modelBuilder);
            ShipperMapping(modelBuilder);
            TaxTypeMapping(modelBuilder);
            RelationshipsMapping(modelBuilder);
        }

        private void CategoryMapping(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().ToTable(@"Categories");
            modelBuilder.Entity<Category>().HasKey(x => x.Id);
            modelBuilder.Entity<Category>().Property(x => x.Name).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Category>().Property(x => x.Description).HasMaxLength(400);
        }

        private void CountryMapping(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Country>().ToTable(@"Countries");
            modelBuilder.Entity<Country>().HasKey(x => x.Id);
            modelBuilder.Entity<Country>().Property(x => x.Code).IsRequired().HasMaxLength(2);
            modelBuilder.Entity<Country>().Property(x => x.Name).IsRequired().HasMaxLength(50);
        }

        private void CustomerMapping(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().ToTable(@"Customers");
            modelBuilder.Entity<Customer>().HasKey(x => x.Id);
            modelBuilder.Entity<Customer>().Property(x => x.Title).HasMaxLength(8);
            modelBuilder.Entity<Customer>().Property(x => x.FirstName).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Customer>().Property(x => x.MiddleName).HasMaxLength(50);
            modelBuilder.Entity<Customer>().Property(x => x.LastName).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Customer>().Property(x => x.Suffix).HasMaxLength(10);
            modelBuilder.Entity<Customer>().Property(x => x.Gender).HasMaxLength(1);
            modelBuilder.Entity<Customer>().Property(x => x.EmailAddress).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Customer>().Property(x => x.AddressLine1).IsRequired().HasMaxLength(120);
            modelBuilder.Entity<Customer>().Property(x => x.AddressLine2).HasMaxLength(120);
            modelBuilder.Entity<Customer>().Property(x => x.City).IsRequired().HasMaxLength(30);
            modelBuilder.Entity<Customer>().Property(x => x.Region).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Customer>().Property(x => x.PostalCode).IsRequired().HasMaxLength(15);
            modelBuilder.Entity<Customer>().Property(x => x.Phone).HasMaxLength(20);
            modelBuilder.Entity<Customer>().Property(x => x.Education).HasMaxLength(40);
            modelBuilder.Entity<Customer>().Property(x => x.Occupation).HasMaxLength(100);
            modelBuilder.Entity<Customer>().Property(x => x.YearlyIncome).HasColumnType(@"decimal(18,2)");
            modelBuilder.Entity<Customer>().Property(x => x.MaritalStatus).HasMaxLength(1);
            modelBuilder.Entity<Customer>().Property(x => x.CreatedOn).IsRequired();
            modelBuilder.Entity<Customer>().Property(x => x.LastModifiedOn).IsRequired();
        }

        private void OrderItemMapping(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderItem>().ToTable(@"OrderItems");
            modelBuilder.Entity<OrderItem>().HasKey(x => x.Id);
            modelBuilder.Entity<OrderItem>().Property(x => x.OrderId).IsRequired();
            modelBuilder.Entity<OrderItem>().Property(x => x.OrderLine).IsRequired();
            modelBuilder.Entity<OrderItem>().Property(x => x.ProductId).IsRequired();
            modelBuilder.Entity<OrderItem>().Property(x => x.Quantity).IsRequired();
            modelBuilder.Entity<OrderItem>().Property(x => x.UnitPrice).HasColumnType(@"decimal(18,2)").IsRequired();
            modelBuilder.Entity<OrderItem>().Property(x => x.Discount).HasColumnType(@"decimal(18,2)").IsRequired();
        }

        private void OrderMapping(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().ToTable(@"Orders");
            modelBuilder.Entity<Order>().HasKey(x => x.Id);
            modelBuilder.Entity<Order>().Property(x => x.CustomerId).IsRequired();
            modelBuilder.Entity<Order>().Property(x => x.OrderDate).IsRequired();
            modelBuilder.Entity<Order>().Property(x => x.TrackingNumber).HasMaxLength(50);
            modelBuilder.Entity<Order>().Property(x => x.ShipAddress).HasMaxLength(120);
            modelBuilder.Entity<Order>().Property(x => x.ShipCity).HasMaxLength(30);
            modelBuilder.Entity<Order>().Property(x => x.ShipRegion).HasMaxLength(50);
            modelBuilder.Entity<Order>().Property(x => x.ShipPostalCode).HasMaxLength(15);
            modelBuilder.Entity<Order>().Property(x => x.ShipPhone).HasMaxLength(20);
            modelBuilder.Entity<Order>().Property(x => x.LastModifiedOn).IsRequired();
        }

        private void OrderStatusMapping(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderStatus>().ToTable(@"OrderStatuses");
            modelBuilder.Entity<OrderStatus>().HasKey(x => x.Id);
            modelBuilder.Entity<OrderStatus>().Property(x => x.Name).IsRequired().HasMaxLength(50);
        }

        private void PaymentTypeMapping(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PaymentType>().ToTable(@"PaymentTypes");
            modelBuilder.Entity<PaymentType>().HasKey(x => x.Id);
            modelBuilder.Entity<PaymentType>().Property(x => x.Name).IsRequired().HasMaxLength(50);
        }

        private void ProductMapping(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().ToTable(@"Products");
            modelBuilder.Entity<Product>().HasKey(x => x.Id);
            modelBuilder.Entity<Product>().Property(x => x.Name).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Product>().Property(x => x.Description).HasMaxLength(1000);
            modelBuilder.Entity<Product>().Property(x => x.Size).HasMaxLength(4);
            modelBuilder.Entity<Product>().Property(x => x.Color).HasMaxLength(50);
            modelBuilder.Entity<Product>().Property(x => x.ListPrice).HasColumnType(@"decimal(18,2)").IsRequired();
            modelBuilder.Entity<Product>().Property(x => x.DealerPrice).HasColumnType(@"decimal(18,2)").IsRequired();
            modelBuilder.Entity<Product>().Property(x => x.Discount).HasColumnType(@"decimal(18,2)").IsRequired();
            modelBuilder.Entity<Product>().Property(x => x.StockUnits).IsRequired();
            modelBuilder.Entity<Product>().Property(x => x.SafetyStockLevel).IsRequired();
            modelBuilder.Entity<Product>().Property(x => x.CreatedOn).IsRequired();
            modelBuilder.Entity<Product>().Property(x => x.LastModifiedOn).IsRequired();
        }

        private void ShipperMapping(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Shipper>().ToTable(@"Shippers");
            modelBuilder.Entity<Shipper>().HasKey(x => x.Id);
            modelBuilder.Entity<Shipper>().Property(x => x.Name).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Shipper>().Property(x => x.Phone).HasMaxLength(20);
        }

        private void TaxTypeMapping(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaxType>().ToTable(@"TaxTypes");
            modelBuilder.Entity<TaxType>().HasKey(x => x.Id);
            modelBuilder.Entity<TaxType>().Property(x => x.Name).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<TaxType>().Property(x => x.Rate).HasColumnType(@"decimal(18,2)").IsRequired();
        }

        private void RelationshipsMapping(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                .HasOne(x => x.Country)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(true)
                .HasForeignKey(x => x.CountryId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(x => x.Order)
                .WithMany(op => op.OrderItems)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(true)
                .HasForeignKey(x => x.OrderId);
            modelBuilder.Entity<OrderItem>()
                .HasOne(x => x.Product)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(true)
                .HasForeignKey(x => x.ProductId);
            modelBuilder.Entity<OrderItem>()
                .HasOne(x => x.TaxType)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction)
                //.OnDelete(DeleteBehavior.Cascade)
                .IsRequired(true)
                .HasForeignKey(x => x.TaxTypeId);

            modelBuilder.Entity<Order>()
                .HasMany(x => x.OrderItems)
                .WithOne(op => op.Order)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(true)
                .HasForeignKey(x => x.OrderId);
            modelBuilder.Entity<Order>()
                .HasOne(x => x.Customer)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(true)
                .HasForeignKey(x => x.CustomerId);
            modelBuilder.Entity<Order>()
                .HasOne(x => x.Status)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(true)
                .HasForeignKey(x => x.StatusId);
            modelBuilder.Entity<Order>()
                .HasOne(x => x.PaymentType)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(true)
                .HasForeignKey(x => x.PaymentTypeId);
            modelBuilder.Entity<Order>()
                .HasOne(x => x.Shipper)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade)
                //.IsRequired(false)
                .HasForeignKey(x => x.ShipperId);
            modelBuilder.Entity<Order>()
                .HasOne(x => x.ShipCountry)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction)
                //.OnDelete(DeleteBehavior.Cascade)
                .IsRequired(true)
                .HasForeignKey(x => x.ShipCountryId);

            modelBuilder.Entity<Product>()
                .HasOne(x => x.Category)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(true)
                .HasForeignKey(x => x.CategoryId);
            modelBuilder.Entity<Product>()
                .HasOne(x => x.TaxType)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(true)
                .HasForeignKey(x => x.TaxTypeId);
        }
    }
}
