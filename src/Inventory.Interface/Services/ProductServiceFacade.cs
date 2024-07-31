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

using Inventory.Application;
using Inventory.Domain.Model;
using Inventory.Infrastructure.Common;
using Inventory.Interface.Dto;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory.Interface.Services
{
    public class ProductServiceFacade
    {
        private readonly ILogger<ProductServiceFacade> _logger;
        private readonly ProductService _productService;

        public ProductServiceFacade(ILogger<ProductServiceFacade> logger,
                                    ProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        public async Task<int> DeleteProductAsync(ProductDto model)
        {
            return await _productService.DeleteProductAsync(model.Id);
        }

        public async Task<int> DeleteProductRangeAsync(int index,
                                                       int length,
                                                       DataRequest<Product> request)
        {
            return await _productService.DeleteProductRangeAsync(index, length, request);
        }

        public async Task<ProductDto> GetProductAsync(long id)
        {
            var item = await _productService.GetProductAsync(id);
            var model = DtoAssembler.DtoFromProduct(item, includeAllFields: true);
            return model;
        }

        public async Task<List<ProductDto>> GetProductsAsync(int skip,
                                                             int take,
                                                             DataRequest<Product> request)
        {
            var models = new List<ProductDto>();
            var items = await _productService.GetProductsAsync(skip, take, request);
            foreach (var item in items)
            {
                var dto = DtoAssembler.DtoFromProduct(item, includeAllFields: false);
                models.Add(dto);
            }
            return models;
        }

        public async Task<int> GetProductsCountAsync(DataRequest<Product> request)
        {
            return await _productService.GetProductsCountAsync(request);
        }

        public async Task<int> UpdateProductAsync(ProductDto model)
        {
            long id = model.Id;
            int rtn = 0;
            Product product = id > 0 ? await _productService.GetProductAsync(model.Id) : new Product();
            if (product != null)
            {
                DtoAssembler.UpdateProductFromDto(product, model);
                rtn = await _productService.UpdateProductAsync(product);
                // TODO: verificare l'effetiva utilità nel'aggiornare l'oggetto nodel
                var item = await _productService.GetProductAsync(id);
                var newmodel = DtoAssembler.DtoFromProduct(item, includeAllFields: true);
                model.Merge(newmodel);
            }
            return rtn;
        }
    }
}
