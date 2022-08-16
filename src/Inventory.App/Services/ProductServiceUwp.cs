using CiccioSoft.Inventory.Application;
using CiccioSoft.Inventory.Domain.Model;
using CiccioSoft.Inventory.Infrastructure.Common;
using CiccioSoft.Inventory.Uwp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CiccioSoft.Inventory.Uwp.Services
{
    public class ProductServiceUwp /*: IProductService*/
    {
        private readonly IProductService productService;

        public ProductServiceUwp(IProductService productService)
        {
            this.productService = productService;
        }


        public Task<int> DeleteProductAsync(ProductModel model)
        {
            var product = new Product { ProductID = model.ProductID };
            return productService.DeleteProductAsync(product);
        }

        public Task<int> DeleteProductRangeAsync(int index, int length, DataRequest<Product> request)
        {
            return productService.DeleteProductRangeAsync(index, length, request);
        }

        public async Task<ProductModel> GetProductAsync(string id)
        {
            var product = await productService.GetProductAsync(id);
            var model = await CreateProductModelAsync(product, includeAllFields: true);
            return model;
        }

        public async Task<IList<ProductModel>> GetProductsAsync(int skip, int take, DataRequest<Product> request)
        {
            var models = new List<ProductModel>();
            var products = await productService.GetProductsAsync(skip, take, request);
            foreach (var item in products)
            {
                models.Add(await CreateProductModelAsync(item, includeAllFields: false));
            }
            return models;
        }

        public Task<int> GetProductsCountAsync(DataRequest<Product> request)
        {
            return productService.GetProductsCountAsync(request);
        }

        public async Task<int> UpdateProductAsync(ProductModel model)
        {
            string id = model.ProductID;
            int rtn = 0;
            Product product = !string.IsNullOrEmpty(id) ? await productService.GetProductAsync(model.ProductID) : new Product();
            if (product != null)
            {
                UpdateProductFromModel(product, model);
                rtn = await productService.UpdateProductAsync(product);
                // TODO: verificare l'effetiva utilità nel'aggiornare l'oggetto nodel
                var item = await productService.GetProductAsync(id);
                var newmodel = await CreateProductModelAsync(item, includeAllFields: true);
                model.Merge(newmodel);
            }
            return rtn;
        }




        public static async Task<ProductModel> CreateProductModelAsync(Product source, bool includeAllFields)
        {
            var model = new ProductModel()
            {
                ProductID = source.ProductID,
                CategoryID = source.CategoryID,
                Name = source.Name,
                Description = source.Description,
                Size = source.Size,
                Color = source.Color,
                ListPrice = source.ListPrice,
                DealerPrice = source.DealerPrice,
                TaxType = source.TaxType,
                Discount = source.Discount,
                DiscountStartDate = source.DiscountStartDate,
                DiscountEndDate = source.DiscountEndDate,
                StockUnits = source.StockUnits,
                SafetyStockLevel = source.SafetyStockLevel,
                CreatedOn = source.CreatedOn,
                LastModifiedOn = source.LastModifiedOn,
                Thumbnail = source.Thumbnail,
                ThumbnailSource = await BitmapTools.LoadBitmapAsync(source.Thumbnail)
            };

            if (includeAllFields)
            {
                model.Picture = source.Picture;
                model.PictureSource = await BitmapTools.LoadBitmapAsync(source.Picture);
            }
            return model;
        }

        private void UpdateProductFromModel(Product target, ProductModel source)
        {
            target.CategoryID = source.CategoryID;
            target.Name = source.Name;
            target.Description = source.Description;
            target.Size = source.Size;
            target.Color = source.Color;
            target.ListPrice = source.ListPrice;
            target.DealerPrice = source.DealerPrice;
            target.TaxType = source.TaxType;
            target.Discount = source.Discount;
            target.DiscountStartDate = source.DiscountStartDate;
            target.DiscountEndDate = source.DiscountEndDate;
            target.StockUnits = source.StockUnits;
            target.SafetyStockLevel = source.SafetyStockLevel;
            target.CreatedOn = source.CreatedOn;
            target.LastModifiedOn = source.LastModifiedOn;
            target.Picture = source.Picture;
            target.Thumbnail = source.Thumbnail;
        }
    }
}
