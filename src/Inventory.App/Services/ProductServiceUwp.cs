using CiccioSoft.Inventory.Data.Models;
using CiccioSoft.Inventory.Data.Services;
using CiccioSoft.Inventory.Domain.Model;
using CiccioSoft.Inventory.Infrastructure.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CiccioSoft.Inventory.Uwp.Services
{
    public class ProductServiceUwp : IProductService
    {
        private readonly IProductService productService;

        public ProductServiceUwp(IProductService productService)
        {
            this.productService = productService;
        }

        public Task<int> DeleteProductAsync(ProductModel model)
        {
            return productService.DeleteProductAsync(model);
        }

        public Task<int> DeleteProductRangeAsync(int index, int length, DataRequest<Product> request)
        {
            return productService.DeleteProductRangeAsync(index, length, request);
        }

        public async Task<ProductModel> GetProductAsync(string id)
        {
            var product = await productService.GetProductAsync(id);
            await CreateProductModelAsync(product, includeAllFields: true);
            return product;
        }

        public async Task<IList<ProductModel>> GetProductsAsync(int skip, int take, DataRequest<Product> request)
        {
            var products = await productService.GetProductsAsync(skip, take, request);
            foreach (var item in products)
            {
                await CreateProductModelAsync(item, includeAllFields: false);
            }
            return products;
        }

        public Task<int> GetProductsCountAsync(DataRequest<Product> request)
        {
            return productService.GetProductsCountAsync(request);
        }

        public async Task<int> UpdateProductAsync(ProductModel model)
        {
            var rtn = await productService.UpdateProductAsync(model);
            await CreateProductModelAsync(model, includeAllFields: true);
            return rtn;
        }



        static public async Task CreateProductModelAsync(ProductModel product, bool includeAllFields)
        {
            product.ThumbnailSource = await BitmapTools.LoadBitmapAsync(product.Thumbnail);
            if (includeAllFields)
                product.PictureSource = await BitmapTools.LoadBitmapAsync(product.Picture);
        }
    }
}
