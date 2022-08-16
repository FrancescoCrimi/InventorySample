﻿using CiccioSoft.Inventory.Domain.Model;
using CiccioSoft.Inventory.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CiccioSoft.Inventory.Domain.Repository
{
    public interface IProductRepository : IDisposable
    {
        Task<Product> GetProductAsync(string id);
        Task<IList<Product>> GetProductsAsync(int skip, int take, DataRequest<Product> request);
        Task<IList<Product>> GetProductKeysAsync(int skip, int take, DataRequest<Product> request);
        Task<int> GetProductsCountAsync(DataRequest<Product> request);
        Task<int> UpdateProductAsync(Product product);
        Task<int> DeleteProductsAsync(params Product[] products);
    }
}