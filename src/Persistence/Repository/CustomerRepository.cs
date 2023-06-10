﻿#region copyright
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
using Inventory.Domain.Repository;
using Inventory.Infrastructure.Common;
using Inventory.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.Persistence.Repository
{
    internal class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _dataSource = null;

        public CustomerRepository(AppDbContext dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<Customer> GetCustomerAsync(long id)
        {
            var cust = await _dataSource.Customers
                .Where(r => r.Id == id)
                .Include(c => c.Country)
                .FirstOrDefaultAsync();
            return cust;
        }

        public async Task<IList<Customer>> GetCustomersAsync(int skip, int take, DataRequest<Customer> request)
        {
            var items = GetCustomers(request);

            // Execute
            var records = await items.Skip(skip).Take(take)
                //.Select(r => new Customer
                //{
                //    Id = r.Id,
                //    Title = r.Title,
                //    FirstName = r.FirstName,
                //    MiddleName = r.MiddleName,
                //    LastName = r.LastName,
                //    Suffix = r.Suffix,
                //    Gender = r.Gender,
                //    EmailAddress = r.EmailAddress,
                //    AddressLine1 = r.AddressLine1,
                //    AddressLine2 = r.AddressLine2,
                //    City = r.City,
                //    Region = r.Region,
                //    Country = r.Country,
                //    PostalCode = r.PostalCode,
                //    Phone = r.Phone,
                //    CreatedOn = r.CreatedOn,
                //    LastModifiedOn = r.LastModifiedOn,
                //    Thumbnail = r.Thumbnail
                //})
                .AsNoTracking()
                .ToListAsync();

            return records;
        }

        public async Task<IList<Customer>> GetCustomerKeysAsync(int skip, int take, DataRequest<Customer> request)
        {
            var items = GetCustomers(request);

            // Execute
            var records = await items.Skip(skip).Take(take)
                .Select(r => new Customer
                {
                    Id = r.Id,
                })
                .AsNoTracking()
                .ToListAsync();

            return records;
        }

        private IQueryable<Customer> GetCustomers(DataRequest<Customer> request)
        {
            IQueryable<Customer> items = _dataSource.Customers;

            // Query
            if (!string.IsNullOrEmpty(request.Query))
            {
                items = items.Where(r => r.SearchTerms.Contains(request.Query.ToLower()));
            }

            // Where
            if (request.Where != null)
            {
                items = items.Where(request.Where);
            }

            // Order By
            if (request.OrderBy != null)
            {
                items = items.OrderBy(request.OrderBy);
            }
            if (request.OrderByDesc != null)
            {
                items = items.OrderByDescending(request.OrderByDesc);
            }

            return items;
        }

        public async Task<int> GetCustomersCountAsync(DataRequest<Customer> request)
        {
            IQueryable<Customer> items = _dataSource.Customers;

            // Query
            if (!string.IsNullOrEmpty(request.Query))
            {
                items = items.Where(r => r.SearchTerms.Contains(request.Query.ToLower()));
            }

            // Where
            if (request.Where != null)
            {
                items = items.Where(request.Where);
            }

            return await items.CountAsync();
        }

        public async Task<int> UpdateCustomerAsync(Customer customer)
        {
            if (customer.Id > 0)
            {
                _dataSource.Entry(customer).State = EntityState.Modified;
            }
            else
            {
                customer.Id = UIDGenerator.Next();
                customer.CreatedOn = DateTime.UtcNow;
                _dataSource.Entry(customer).State = EntityState.Added;
            }
            customer.LastModifiedOn = DateTime.UtcNow;
            customer.SearchTerms = customer.BuildSearchTerms();
            var res = await _dataSource.SaveChangesAsync();
            return res;
        }

        public async Task<int> DeleteCustomersAsync(params Customer[] customers)
        {
            _dataSource.Customers.RemoveRange(customers);
            return await _dataSource.SaveChangesAsync();
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
                if (_dataSource != null)
                {
                    _dataSource.Dispose();
                }
            }
        }
        #endregion
    }
}
