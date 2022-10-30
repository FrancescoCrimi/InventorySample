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
        public DbSet<CountryCode> CountryCodes
        {
            get; set;
        }
        public DbSet<OrderStatus> OrderStatus
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
    }
}
