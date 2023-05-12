#region copyright
// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************
#endregion

using Inventory.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Persistence.DbContexts
{
    public abstract class AppDbContext : DbContext
    {
        protected AppDbContext()
        {
        }

        protected AppDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<DbVersion> DbVersion
        {
            get; set;
        }
        public DbSet<Category> Categories
        {
            get; set;
        }
        public DbSet<Country> Countries
        {
            get; set;
        }
        public DbSet<OrderStatus> OrderStatuses
        {
            get; set;
        }
        public DbSet<PaymentType> PaymentTypes
        {
            get; set;
        }
        public DbSet<Shipper> Shippers
        {
            get; set;
        }
        public DbSet<TaxType> TaxTypes
        {
            get; set;
        }
        public DbSet<Customer> Customers
        {
            get; set;
        }
        public DbSet<Order> Orders
        {
            get; set;
        }
        public DbSet<OrderItem> OrderItems
        {
            get; set;
        }
        public DbSet<Product> Products
        {
            get; set;
        }

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

            ////modelBuilder.Entity<OrderItem>().HasKey(e => new { e.OrderID, e.OrderLine });
            //modelBuilder.Entity<OrderItem>().HasIndex(e => new { e.OrderId, e.OrderLine }).IsUnique();
        }


        private void CategoryMapping(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().ToTable(@"Categories");
            modelBuilder.Entity<Category>().Property(x => x.Id).HasColumnName(@"Id").IsRequired().ValueGeneratedNever();
            modelBuilder.Entity<Category>().Property(x => x.Name).HasColumnName(@"Name").IsRequired().ValueGeneratedNever().HasMaxLength(50);
            modelBuilder.Entity<Category>().Property(x => x.Description).HasColumnName(@"Description").ValueGeneratedNever().HasMaxLength(400);
            modelBuilder.Entity<Category>().Property(x => x.Picture).HasColumnName(@"Picture").ValueGeneratedNever();
            modelBuilder.Entity<Category>().Property(x => x.Thumbnail).HasColumnName(@"Thumbnail").ValueGeneratedNever();
            modelBuilder.Entity<Category>().HasKey(@"Id");
        }

        private void CountryMapping(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Country>().ToTable(@"Countries");
            modelBuilder.Entity<Country>().Property(x => x.Id).HasColumnName(@"Id").IsRequired().ValueGeneratedNever();
            modelBuilder.Entity<Country>().Property(x => x.Code).HasColumnName(@"Code").IsRequired().ValueGeneratedNever().HasMaxLength(2);
            modelBuilder.Entity<Country>().Property(x => x.Name).HasColumnName(@"Name").IsRequired().ValueGeneratedNever().HasMaxLength(50);
            modelBuilder.Entity<Country>().HasKey(@"Id");
        }

        private void CustomerMapping(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().ToTable(@"Customers");
            modelBuilder.Entity<Customer>().Property(x => x.Id).HasColumnName(@"Id").IsRequired().ValueGeneratedNever();
            modelBuilder.Entity<Customer>().Property(x => x.Title).HasColumnName(@"Title").ValueGeneratedNever().HasMaxLength(8);
            modelBuilder.Entity<Customer>().Property(x => x.FirstName).HasColumnName(@"FirstName").IsRequired().ValueGeneratedNever().HasMaxLength(50);
            modelBuilder.Entity<Customer>().Property(x => x.MiddleName).HasColumnName(@"MiddleName").ValueGeneratedNever().HasMaxLength(50);
            modelBuilder.Entity<Customer>().Property(x => x.LastName).HasColumnName(@"LastName").IsRequired().ValueGeneratedNever().HasMaxLength(50);
            modelBuilder.Entity<Customer>().Property(x => x.Suffix).HasColumnName(@"Suffix").ValueGeneratedNever().HasMaxLength(10);
            modelBuilder.Entity<Customer>().Property(x => x.Gender).HasColumnName(@"Gender").ValueGeneratedNever().HasMaxLength(1);
            modelBuilder.Entity<Customer>().Property(x => x.EmailAddress).HasColumnName(@"EmailAddress").IsRequired().ValueGeneratedNever().HasMaxLength(50);
            modelBuilder.Entity<Customer>().Property(x => x.AddressLine1).HasColumnName(@"AddressLine1").IsRequired().ValueGeneratedNever().HasMaxLength(120);
            modelBuilder.Entity<Customer>().Property(x => x.AddressLine2).HasColumnName(@"AddressLine2").ValueGeneratedNever().HasMaxLength(120);
            modelBuilder.Entity<Customer>().Property(x => x.City).HasColumnName(@"City").IsRequired().ValueGeneratedNever().HasMaxLength(30);
            modelBuilder.Entity<Customer>().Property(x => x.Region).HasColumnName(@"Region").IsRequired().ValueGeneratedNever().HasMaxLength(50);
            modelBuilder.Entity<Customer>().Property(x => x.PostalCode).HasColumnName(@"PostalCode").IsRequired().ValueGeneratedNever().HasMaxLength(15);
            modelBuilder.Entity<Customer>().Property(x => x.Phone).HasColumnName(@"Phone").ValueGeneratedNever().HasMaxLength(20);
            modelBuilder.Entity<Customer>().Property(x => x.BirthDate).HasColumnName(@"BirthDate").ValueGeneratedNever();
            modelBuilder.Entity<Customer>().Property(x => x.Education).HasColumnName(@"Education").ValueGeneratedNever().HasMaxLength(40);
            modelBuilder.Entity<Customer>().Property(x => x.Occupation).HasColumnName(@"Occupation").ValueGeneratedNever().HasMaxLength(100);
            modelBuilder.Entity<Customer>().Property(x => x.YearlyIncome).HasColumnName(@"YearlyIncome").HasColumnType(@"decimal(18,2)").ValueGeneratedNever();
            modelBuilder.Entity<Customer>().Property(x => x.MaritalStatus).HasColumnName(@"MaritalStatus").ValueGeneratedNever().HasMaxLength(1);
            modelBuilder.Entity<Customer>().Property(x => x.TotalChildren).HasColumnName(@"TotalChildren").ValueGeneratedNever();
            modelBuilder.Entity<Customer>().Property(x => x.ChildrenAtHome).HasColumnName(@"ChildrenAtHome").ValueGeneratedNever();
            modelBuilder.Entity<Customer>().Property(x => x.IsHouseOwner).HasColumnName(@"IsHouseOwner").ValueGeneratedNever();
            modelBuilder.Entity<Customer>().Property(x => x.NumberCarsOwned).HasColumnName(@"NumberCarsOwned").ValueGeneratedNever();
            modelBuilder.Entity<Customer>().Property(x => x.CreatedOn).HasColumnName(@"CreatedOn").IsRequired().ValueGeneratedNever();
            modelBuilder.Entity<Customer>().Property(x => x.LastModifiedOn).HasColumnName(@"LastModifiedOn").IsRequired().ValueGeneratedNever();
            modelBuilder.Entity<Customer>().Property(x => x.SearchTerms).HasColumnName(@"SearchTerms").ValueGeneratedNever();
            modelBuilder.Entity<Customer>().Property(x => x.Picture).HasColumnName(@"Picture").ValueGeneratedNever();
            modelBuilder.Entity<Customer>().Property(x => x.Thumbnail).HasColumnName(@"Thumbnail").ValueGeneratedNever();
            modelBuilder.Entity<Customer>().Property(x => x.CountryId).HasColumnName(@"CountryId").ValueGeneratedNever();
            modelBuilder.Entity<Customer>().HasKey(@"Id");
        }

        private void OrderItemMapping(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderItem>().ToTable(@"OrderItems");
            modelBuilder.Entity<OrderItem>().Property(x => x.Id).HasColumnName(@"Id").IsRequired().ValueGeneratedNever();
            modelBuilder.Entity<OrderItem>().Property(x => x.OrderId).HasColumnName(@"OrderId").IsRequired().ValueGeneratedNever();
            modelBuilder.Entity<OrderItem>().Property(x => x.OrderLine).HasColumnName(@"OrderLine").IsRequired().ValueGeneratedNever();
            modelBuilder.Entity<OrderItem>().Property(x => x.ProductId).HasColumnName(@"ProductId").IsRequired().ValueGeneratedNever();
            modelBuilder.Entity<OrderItem>().Property(x => x.Quantity).HasColumnName(@"Quantity").IsRequired().ValueGeneratedNever();
            modelBuilder.Entity<OrderItem>().Property(x => x.UnitPrice).HasColumnName(@"UnitPrice").HasColumnType(@"decimal(18,2)").IsRequired().ValueGeneratedNever();
            modelBuilder.Entity<OrderItem>().Property(x => x.Discount).HasColumnName(@"Discount").HasColumnType(@"decimal(18,2)").IsRequired().ValueGeneratedNever();
            modelBuilder.Entity<OrderItem>().Property(x => x.TaxTypeId).HasColumnName(@"TaxTypeId").ValueGeneratedNever();
            modelBuilder.Entity<OrderItem>().HasKey(@"Id");
            modelBuilder.Entity<OrderItem>().HasIndex(@"OrderId", @"OrderLine").IsUnique(true).HasName(@"IX_OrderItems_OrderID_OrderLine");
        }

        private void OrderMapping(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().ToTable(@"Orders");
            modelBuilder.Entity<Order>().Property(x => x.Id).HasColumnName(@"Id").IsRequired().ValueGeneratedNever();
            modelBuilder.Entity<Order>().Property(x => x.CustomerId).HasColumnName(@"CustomerId").IsRequired().ValueGeneratedNever();
            modelBuilder.Entity<Order>().Property(x => x.OrderDate).HasColumnName(@"OrderDate").IsRequired().ValueGeneratedNever();
            modelBuilder.Entity<Order>().Property(x => x.ShippedDate).HasColumnName(@"ShippedDate").ValueGeneratedNever();
            modelBuilder.Entity<Order>().Property(x => x.DeliveredDate).HasColumnName(@"DeliveredDate").ValueGeneratedNever();
            modelBuilder.Entity<Order>().Property(x => x.TrackingNumber).HasColumnName(@"TrackingNumber").ValueGeneratedNever().HasMaxLength(50);
            modelBuilder.Entity<Order>().Property(x => x.ShipAddress).HasColumnName(@"ShipAddress").ValueGeneratedNever().HasMaxLength(120);
            modelBuilder.Entity<Order>().Property(x => x.ShipCity).HasColumnName(@"ShipCity").ValueGeneratedNever().HasMaxLength(30);
            modelBuilder.Entity<Order>().Property(x => x.ShipRegion).HasColumnName(@"ShipRegion").ValueGeneratedNever().HasMaxLength(50);
            modelBuilder.Entity<Order>().Property(x => x.ShipPostalCode).HasColumnName(@"ShipPostalCode").ValueGeneratedNever().HasMaxLength(15);
            modelBuilder.Entity<Order>().Property(x => x.ShipPhone).HasColumnName(@"ShipPhone").ValueGeneratedNever().HasMaxLength(20);
            modelBuilder.Entity<Order>().Property(x => x.LastModifiedOn).HasColumnName(@"LastModifiedOn").IsRequired().ValueGeneratedNever();
            modelBuilder.Entity<Order>().Property(x => x.SearchTerms).HasColumnName(@"SearchTerms").ValueGeneratedNever();
            modelBuilder.Entity<Order>().Property(x => x.StatusId).HasColumnName(@"StatusId").ValueGeneratedNever();
            modelBuilder.Entity<Order>().Property(x => x.PaymentTypeId).HasColumnName(@"PaymentTypeId").ValueGeneratedNever();
            modelBuilder.Entity<Order>().Property(x => x.ShipperId).HasColumnName(@"ShipperId").ValueGeneratedNever();
            modelBuilder.Entity<Order>().Property(x => x.ShipCountryId).HasColumnName(@"ShipCountryId").ValueGeneratedNever();
            modelBuilder.Entity<Order>().HasKey(@"Id");
        }

        private void OrderStatusMapping(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderStatus>().ToTable(@"OrderStatuses");
            modelBuilder.Entity<OrderStatus>().Property(x => x.Id).HasColumnName(@"Id").IsRequired().ValueGeneratedNever();
            modelBuilder.Entity<OrderStatus>().Property(x => x.Name).HasColumnName(@"Name").IsRequired().ValueGeneratedNever().HasMaxLength(50);
            modelBuilder.Entity<OrderStatus>().HasKey(@"Id");
        }

        private void PaymentTypeMapping(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PaymentType>().ToTable(@"PaymentTypes");
            modelBuilder.Entity<PaymentType>().Property(x => x.Id).HasColumnName(@"Id").IsRequired().ValueGeneratedNever();
            modelBuilder.Entity<PaymentType>().Property(x => x.Name).HasColumnName(@"Name").IsRequired().ValueGeneratedNever().HasMaxLength(50);
            modelBuilder.Entity<PaymentType>().HasKey(@"Id");
        }

        private void ProductMapping(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().ToTable(@"Products");
            modelBuilder.Entity<Product>().Property(x => x.Id).HasColumnName(@"Id").IsRequired().ValueGeneratedNever();
            modelBuilder.Entity<Product>().Property(x => x.Name).HasColumnName(@"Name").IsRequired().ValueGeneratedNever().HasMaxLength(50);
            modelBuilder.Entity<Product>().Property(x => x.Description).HasColumnName(@"Description").ValueGeneratedNever().HasMaxLength(1000);
            modelBuilder.Entity<Product>().Property(x => x.Size).HasColumnName(@"Size").ValueGeneratedNever().HasMaxLength(4);
            modelBuilder.Entity<Product>().Property(x => x.Color).HasColumnName(@"Color").ValueGeneratedNever().HasMaxLength(50);
            modelBuilder.Entity<Product>().Property(x => x.ListPrice).HasColumnName(@"ListPrice").HasColumnType(@"decimal(18,2)").IsRequired().ValueGeneratedNever();
            modelBuilder.Entity<Product>().Property(x => x.DealerPrice).HasColumnName(@"DealerPrice").HasColumnType(@"decimal(18,2)").IsRequired().ValueGeneratedNever();
            modelBuilder.Entity<Product>().Property(x => x.Discount).HasColumnName(@"Discount").HasColumnType(@"decimal(18,2)").IsRequired().ValueGeneratedNever();
            modelBuilder.Entity<Product>().Property(x => x.DiscountStartDate).HasColumnName(@"DiscountStartDate").ValueGeneratedNever();
            modelBuilder.Entity<Product>().Property(x => x.DiscountEndDate).HasColumnName(@"DiscountEndDate").ValueGeneratedNever();
            modelBuilder.Entity<Product>().Property(x => x.StockUnits).HasColumnName(@"StockUnits").IsRequired().ValueGeneratedNever();
            modelBuilder.Entity<Product>().Property(x => x.SafetyStockLevel).HasColumnName(@"SafetyStockLevel").IsRequired().ValueGeneratedNever();
            modelBuilder.Entity<Product>().Property(x => x.CreatedOn).HasColumnName(@"CreatedOn").IsRequired().ValueGeneratedNever();
            modelBuilder.Entity<Product>().Property(x => x.LastModifiedOn).HasColumnName(@"LastModifiedOn").IsRequired().ValueGeneratedNever();
            modelBuilder.Entity<Product>().Property(x => x.SearchTerms).HasColumnName(@"SearchTerms").ValueGeneratedNever();
            modelBuilder.Entity<Product>().Property(x => x.Picture).HasColumnName(@"Picture").ValueGeneratedNever();
            modelBuilder.Entity<Product>().Property(x => x.Thumbnail).HasColumnName(@"Thumbnail").ValueGeneratedNever();
            modelBuilder.Entity<Product>().Property(x => x.CategoryId).HasColumnName(@"CategoryId").ValueGeneratedNever();
            modelBuilder.Entity<Product>().Property(x => x.TaxTypeId).HasColumnName(@"TaxTypeId").ValueGeneratedNever();
            modelBuilder.Entity<Product>().HasKey(@"Id");
        }

        private void ShipperMapping(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Shipper>().ToTable(@"Shippers");
            modelBuilder.Entity<Shipper>().Property(x => x.Id).HasColumnName(@"Id").IsRequired().ValueGeneratedNever();
            modelBuilder.Entity<Shipper>().Property(x => x.Name).HasColumnName(@"Name").IsRequired().ValueGeneratedNever().HasMaxLength(50);
            modelBuilder.Entity<Shipper>().Property(x => x.Phone).HasColumnName(@"Phone").ValueGeneratedNever().HasMaxLength(20);
            modelBuilder.Entity<Shipper>().HasKey(@"Id");
        }

        private void TaxTypeMapping(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaxType>().ToTable(@"TaxTypes");
            modelBuilder.Entity<TaxType>().Property(x => x.Id).HasColumnName(@"Id").IsRequired().ValueGeneratedNever();
            modelBuilder.Entity<TaxType>().Property(x => x.Name).HasColumnName(@"Name").IsRequired().ValueGeneratedNever().HasMaxLength(50);
            modelBuilder.Entity<TaxType>().Property(x => x.Rate).HasColumnName(@"Rate").HasColumnType(@"decimal(18,2)").IsRequired().ValueGeneratedNever();
            modelBuilder.Entity<TaxType>().HasKey(@"Id");
        }

        private void RelationshipsMapping(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().HasMany(x => x.Orders).WithOne(op => op.Customer).OnDelete(DeleteBehavior.Cascade).IsRequired(true).HasForeignKey(@"CustomerId");
            modelBuilder.Entity<Customer>().HasOne(x => x.Country).WithMany().OnDelete(DeleteBehavior.Cascade).IsRequired(true).HasForeignKey(@"CountryId");

            modelBuilder.Entity<OrderItem>().HasOne(x => x.Order).WithMany(op => op.OrderItems).OnDelete(DeleteBehavior.Cascade).IsRequired(true).HasForeignKey(@"OrderId");
            modelBuilder.Entity<OrderItem>().HasOne(x => x.Product).WithMany().OnDelete(DeleteBehavior.Cascade).IsRequired(true).HasForeignKey(@"ProductId");
            modelBuilder.Entity<OrderItem>().HasOne(x => x.TaxType).WithMany().OnDelete(DeleteBehavior.NoAction).IsRequired(true).HasForeignKey(@"TaxTypeId");
            //modelBuilder.Entity<OrderItem>().HasOne(x => x.TaxType).WithMany().OnDelete(DeleteBehavior.Cascade).IsRequired(true).HasForeignKey(@"TaxTypeId");

            modelBuilder.Entity<Order>().HasMany(x => x.OrderItems).WithOne(op => op.Order).OnDelete(DeleteBehavior.Cascade).IsRequired(true).HasForeignKey(@"OrderId");
            modelBuilder.Entity<Order>().HasOne(x => x.Customer).WithMany(op => op.Orders).OnDelete(DeleteBehavior.Cascade).IsRequired(true).HasForeignKey(@"CustomerId");
            modelBuilder.Entity<Order>().HasOne(x => x.Status).WithMany().OnDelete(DeleteBehavior.Cascade).IsRequired(true).HasForeignKey(@"StatusId");
            modelBuilder.Entity<Order>().HasOne(x => x.PaymentType).WithMany().OnDelete(DeleteBehavior.Cascade).IsRequired(true).HasForeignKey(@"PaymentTypeId");
            modelBuilder.Entity<Order>().HasOne(x => x.Shipper).WithMany().OnDelete(DeleteBehavior.Cascade).IsRequired(true).HasForeignKey(@"ShipperId");
            modelBuilder.Entity<Order>().HasOne(x => x.ShipCountry).WithMany().OnDelete(DeleteBehavior.NoAction).IsRequired(true).HasForeignKey(@"ShipCountryId");
            //modelBuilder.Entity<Order>().HasOne(x => x.ShipCountry).WithMany().OnDelete(DeleteBehavior.Cascade).IsRequired(true).HasForeignKey(@"ShipCountryId");

            modelBuilder.Entity<Product>().HasOne(x => x.Category).WithMany().OnDelete(DeleteBehavior.Cascade).IsRequired(true).HasForeignKey(@"CategoryId");
            modelBuilder.Entity<Product>().HasOne(x => x.TaxType).WithMany().OnDelete(DeleteBehavior.Cascade).IsRequired(true).HasForeignKey(@"TaxTypeId");
        }
    }
}
