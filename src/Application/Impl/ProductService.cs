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

using Inventory.Application;
using Inventory.Domain.Model;
using Inventory.Domain.Repository;
using Inventory.Infrastructure.Common;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.Application.Impl
{
    public class ProductService : IProductService
    {
        private readonly IServiceProvider serviceProvider;

        public ProductService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task<Product> GetProductAsync(string id)
        {
            using (var repo = serviceProvider.GetService<IProductRepository>())
            {
                var item = await repo.GetProductAsync(id);
                return item;
            }
        }

        public async Task<IList<Product>> GetProductsAsync(int skip, int take, DataRequest<Product> request)
        {
            using (var repo = serviceProvider.GetService<IProductRepository>())
            {
                var items = await repo.GetProductsAsync(skip, take, request);
                return items;
            }
        }

        public async Task<int> GetProductsCountAsync(DataRequest<Product> request)
        {
            using (var repo = serviceProvider.GetService<IProductRepository>())
            {
                return await repo.GetProductsCountAsync(request);
            }
        }

        public async Task<int> UpdateProductAsync(Product product)
        {
            using (var repo = serviceProvider.GetService<IProductRepository>())
            {
                return await repo.UpdateProductAsync(product);
            }
        }

        public async Task<int> DeleteProductAsync(Product product)
        {
            using (var repo = serviceProvider.GetService<IProductRepository>())
            {
                return await repo.DeleteProductsAsync(product);
            }
        }

        public async Task<int> DeleteProductRangeAsync(int index, int length, DataRequest<Product> request)
        {
            using (var repo = serviceProvider.GetService<IProductRepository>())
            {
                var items = await repo.GetProductKeysAsync(index, length, request);
                return await repo.DeleteProductsAsync(items.ToArray());
            }
        }
    }
}
