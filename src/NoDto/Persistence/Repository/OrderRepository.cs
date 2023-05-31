// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.

using Inventory.Domain.Aggregates.OrderAggregate;
using Inventory.Infrastructure.Common;
using Inventory.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.Persistence.Repository
{
    internal class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _dbContext = null;

        public OrderRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IList<Order>> GetOrdersAsync(int skip, int take, DataRequest<Order> request)
        {
            var items = GetOrders(request);

            // Execute
            var records = await items.Skip(skip).Take(take)
                //.AsNoTracking()
                .ToListAsync();

            return records;
        }

        public async Task<IList<Order>> GetOrderKeysAsync(int skip, int take, DataRequest<Order> request)
        {
            var items = GetOrders(request);

            // Execute
            var records = await items.Skip(skip).Take(take)
                .Select(r => new Order
                {
                    Id = r.Id,
                })
                .AsNoTracking()
                .ToListAsync();

            return records;
        }

        public async Task<int> GetOrdersCountAsync(DataRequest<Order> request)
        {
            IQueryable<Order> items = _dbContext.Orders;

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

        public async Task<Order> GetOrderAsync(long id)
        {
            return await _dbContext.Orders.Where(r => r.Id == id)
                //.Include(r => r.Customer)
                //.Include(o => o.Status)
                //.Include(o => o.ShipCountry)
                .FirstOrDefaultAsync();
        }

        public async Task<int> UpdateOrderAsync(Order order)
        {
            if (order.Id > 0)
            {
                _dbContext.Entry(order).State = EntityState.Modified;
            }
            else
            {
                order.Id = UIDGenerator.Next(4);
                order.OrderDate = DateTime.UtcNow;
                _dbContext.Entry(order).State = EntityState.Added;
            }
            order.LastModifiedOn = DateTime.UtcNow;
            order.SearchTerms = order.BuildSearchTerms();
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteOrdersAsync(params Order[] orders)
        {
            _dbContext.Orders.RemoveRange(orders);
            return await _dbContext.SaveChangesAsync();
        }


        public async Task<List<OrderStatus>> GetOrderStatusAsync()
        {
            return await _dbContext.OrderStatuses.AsNoTracking().ToListAsync();
        }

        public async Task<List<PaymentType>> GetPaymentTypesAsync()
        {
            return await _dbContext.PaymentTypes.AsNoTracking().ToListAsync();
        }

        public async Task<List<Shipper>> GetShippersAsync()
        {
            return await _dbContext.Shippers.AsNoTracking().ToListAsync();
        }


        private IQueryable<Order> GetOrders(DataRequest<Order> request)
        {
            IQueryable<Order> items = _dbContext.Orders;

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
