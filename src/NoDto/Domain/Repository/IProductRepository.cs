using Inventory.Domain.Model;
using Inventory.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory.Domain.Repository
{
    public interface IProductRepository : IDisposable
    {
        Task<Product> GetProductAsync(long id);
        Task<IList<Product>> GetProductsAsync(int skip, int take, DataRequest<Product> request);
        Task<IList<Product>> GetProductKeysAsync(int skip, int take, DataRequest<Product> request);
        Task<int> GetProductsCountAsync(DataRequest<Product> request);
        Task<int> UpdateProductAsync(Product product);
        Task<int> DeleteProductsAsync(params Product[] products);
    }
}
