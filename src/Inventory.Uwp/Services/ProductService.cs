﻿using Inventory.Domain.Model;
using Inventory.Domain.Repository;
using Inventory.Infrastructure.Common;
using Inventory.Uwp.Dto;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.Uwp.Services
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

        public async Task<int> DeleteProductAsync(ProductDto model)
        {
            using (var productRepository = _serviceProvider.GetService<IProductRepository>())
            {
                var product = await productRepository.GetProductAsync(model.Id);
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

        public async Task<ProductDto> GetProductAsync(long id)
        {
            using (var productRepository = _serviceProvider.GetService<IProductRepository>())
            {
                var item = await productRepository.GetProductAsync(id);
                var model = DtoAssembler.DtoFromProduct(item, includeAllFields: true);
                return model;
            }
        }

        public async Task<List<ProductDto>> GetProductsAsync(int skip,
                                                             int take,
                                                             DataRequest<Product> request,
                                                             Windows.UI.Core.CoreDispatcher dispatcher = null)
        {
            using (var productRepository = _serviceProvider.GetService<IProductRepository>())
            {
                var models = new List<ProductDto>();
                var items = await productRepository.GetProductsAsync(skip, take, request);
                foreach (var item in items)
                {
                    var dto = DtoAssembler.DtoFromProduct(item, includeAllFields: false);
                    models.Add(dto);
                }
                return models;
            }
        }

        public async Task<int> GetProductsCountAsync(DataRequest<Product> request)
        {
            using (var productRepository = _serviceProvider.GetService<IProductRepository>())
            {
                return await productRepository.GetProductsCountAsync(request);
            }
        }

        public async Task<int> UpdateProductAsync(ProductDto model)
        {
            using (var productRepository = _serviceProvider.GetService<IProductRepository>())
            {
                long id = model.Id;
                int rtn = 0;
                Product product = id > 0 ? await productRepository.GetProductAsync(model.Id) : new Product();
                if (product != null)
                {
                    DtoAssembler.UpdateProductFromDto(product, model);
                    rtn = await productRepository.UpdateProductAsync(product);
                    // TODO: verificare l'effetiva utilità nel'aggiornare l'oggetto nodel
                    var item = await productRepository.GetProductAsync(id);
                    var newmodel = DtoAssembler.DtoFromProduct(item, includeAllFields: true);
                    model.Merge(newmodel);
                }
                return rtn;
            }
        }
    }
}
