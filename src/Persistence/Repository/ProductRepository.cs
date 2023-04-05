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
    internal class ProductRepository : IProductRepository
    {
        private AppDbContext _dataSource = null;

        public ProductRepository(AppDbContext dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<Product> GetProductAsync(long id)
        {
            return await _dataSource.Products.Where(r => r.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IList<Product>> GetProductsAsync(int skip, int take, DataRequest<Product> request)
        {
            var items = GetProducts(request);

            // Execute
            var records = await items.Skip(skip).Take(take)
                .AsNoTracking()
                .ToListAsync();

            return records;
        }

        public async Task<IList<Product>> GetProductKeysAsync(int skip, int take, DataRequest<Product> request)
        {
            var items = GetProducts(request);

            // Execute
            var records = await items.Skip(skip).Take(take)
                .Select(r => new Product
                {
                    Id = r.Id,
                })
                .AsNoTracking()
                .ToListAsync();

            return records;
        }

        private IQueryable<Product> GetProducts(DataRequest<Product> request)
        {
            IQueryable<Product> items = _dataSource.Products;

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

        public async Task<int> GetProductsCountAsync(DataRequest<Product> request)
        {
            IQueryable<Product> items = _dataSource.Products;

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

        public async Task<int> UpdateProductAsync(Product product)
        {
            if (product.Id > 0)
            {
                _dataSource.Entry(product).State = EntityState.Modified;
            }
            else
            {
                product.Id = UIDGenerator.Next(6);
                product.CreatedOn = DateTime.UtcNow;
                _dataSource.Entry(product).State = EntityState.Added;
            }
            product.LastModifiedOn = DateTime.UtcNow;
            product.SearchTerms = product.BuildSearchTerms();
            return await _dataSource.SaveChangesAsync();
        }

        public async Task<int> DeleteProductsAsync(params Product[] products)
        {
            _dataSource.Products.RemoveRange(products);
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
