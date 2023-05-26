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
using Inventory.Infrastructure.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application
{
    public class ProductService
    {
        private readonly ILogger<ProductService> _logger;
        private readonly IProductRepository _productRepository;
        private static List<Category> _categories;
        private static List<TaxType> _taxTypes;

        public ProductService(ILogger<ProductService> logger,
                              IProductRepository productRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
            Task.Run(async() =>
            {
                await GetCategoriesAsync();
                await GetTaxTypesAsync();
            });
        }

        public IEnumerable<Category> Categories => _categories;
        public IEnumerable<TaxType> TaxTypes => _taxTypes;


        private async Task GetCategoriesAsync()
        {
            if(_categories == null)
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
