// Copyright (c) 2023 Francesco Crimi francrim@gmail.com
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.

using Inventory.Domain.Aggregates.ProductAggregate;
using Inventory.Infrastructure.Common;
using Inventory.Infrastructure.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application
{
    public class ProductService
    {
        private readonly ILogger _logger;
        private readonly IProductRepository _productRepository;
        private static List<Category> _categories;
        private static List<TaxType> _taxTypes;

        public ProductService(ILogger<ProductService> logger,
                              IProductRepository productRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
            Task.Run(async () =>
            {
                await GetCategoriesAsync();
                await GetTaxTypesAsync();
            });
        }

        public IEnumerable<Category> Categories => _categories;

        public IEnumerable<TaxType> TaxTypes => _taxTypes;


        public async Task<IList<Product>> GetProductsAsync(int index, int length, DataRequest<Product> request)
        {
            return await _productRepository.GetProductsAsync(index, length, request);
        }

        public async Task<int> GetProductsCountAsync(DataRequest<Product> request)
        {
            return await _productRepository.GetProductsCountAsync(request);
        }

        public async Task<Product> GetProductAsync(long productId)
        {
            return await _productRepository.GetProductAsync(productId);
        }

        public async Task UpdateProductAsync(Product model)
        {
            await _productRepository.UpdateProductAsync(model);
        }

        public async Task DeleteProductsAsync(Product model)
        {
            await _productRepository.DeleteProductsAsync(model);
        }

        public async Task DeleteProductRangeAsync(int index, int length, DataRequest<Product> request)
        {
            var items = await _productRepository.GetProductKeysAsync(index, length, request);
            await _productRepository.DeleteProductsAsync(items.ToArray());
        }


        private async Task GetCategoriesAsync()
        {
            if (_categories == null)
            {
                try
                {
                    _categories = await _productRepository.GetCategoriesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(LogEvents.LoadCategories, ex, "Load Categories");
                }
            }
        }

        private async Task GetTaxTypesAsync()
        {
            if (_taxTypes == null)
            {
                try
                {
                    _taxTypes = await _productRepository.GetTaxTypesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(LogEvents.LoadTaxTypes, ex, "Load TaxTypes");
                }
            }
        }
    }
}
