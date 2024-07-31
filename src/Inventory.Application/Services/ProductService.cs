// Copyright (c) Microsoft. All rights reserved.
// Copyright (c) 2023 Francesco Crimi francrim@gmail.com
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.

using Inventory.Domain.Model;
using Inventory.Domain.Repository;
using Inventory.Infrastructure.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.Application
{
    public class ProductService
    {
        private readonly ILogger<ProductService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public ProductService(ILogger<ProductService> logger,
                                    IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task<int> DeleteProductAsync(long ProductId)
        {
            using (var productRepository = _serviceProvider.GetService<IProductRepository>())
            {
                var product = await productRepository.GetProductAsync(ProductId);
                return await productRepository.DeleteProductsAsync(product);
            }
        }

        public async Task<int> DeleteProductRangeAsync(int index,
                                                       int length,
                                                       DataRequest<Product> request)
        {
            using (var productRepository = _serviceProvider.GetService<IProductRepository>())
            {
                var items = await productRepository.GetProductKeysAsync(index, length, request);
                return await productRepository.DeleteProductsAsync(items.ToArray());
            }
        }

        public async Task<Product> GetProductAsync(long id)
        {
            using (var productRepository = _serviceProvider.GetService<IProductRepository>())
            {
                var item = await productRepository.GetProductAsync(id);
                //var model = DtoAssembler.DtoFromProduct(item, includeAllFields: true);
                return item;
            }
        }

        public async Task<IList<Product>> GetProductsAsync(int skip,
                                                             int take,
                                                             DataRequest<Product> request
            //, Windows.UI.Core.CoreDispatcher dispatcher = null
            )
        {
            using (var productRepository = _serviceProvider.GetService<IProductRepository>())
            {
                //var models = new List<ProductDto>();
                var items = await productRepository.GetProductsAsync(skip, take, request);
                //foreach (var item in items)
                //{
                //    var dto = DtoAssembler.DtoFromProduct(item, includeAllFields: false);
                //    models.Add(dto);
                //}
                return items;
            }
        }

        public async Task<int> GetProductsCountAsync(DataRequest<Product> request)
        {
            using (var productRepository = _serviceProvider.GetService<IProductRepository>())
            {
                return await productRepository.GetProductsCountAsync(request);
            }
        }

        public async Task<int> UpdateProductAsync(Product product)
        {
            using (var productRepository = _serviceProvider.GetService<IProductRepository>())
            {
                //long id = model.Id;
                int rtn = 0;
                //Product product = id > 0 ? await productRepository.GetProductAsync(model.Id) : new Product();
                //if (product != null)
                //{
                //    DtoAssembler.UpdateProductFromDto(product, model);
                    rtn = await productRepository.UpdateProductAsync(product);
                    // TODO: verificare l'effetiva utilità nel'aggiornare l'oggetto nodel
                    //var item = await productRepository.GetProductAsync(id);
                //    var newmodel = DtoAssembler.DtoFromProduct(item, includeAllFields: true);
                //    model.Merge(newmodel);
                //}
                return rtn;
            }
        }
    }
}
