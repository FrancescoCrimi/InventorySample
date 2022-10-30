using Inventory.Application;
using Inventory.Domain.Model;
using Inventory.Infrastructure.Common;
using Inventory.UwpApp.Dto;
using Inventory.UwpApp.Tools;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory.UwpApp.Services
{
    public class ProductServiceFacade /*: IProductService*/
    {
        private readonly IProductService productService;

        public ProductServiceFacade(IProductService productService)
        {
            this.productService = productService;
        }


        public Task<int> DeleteProductAsync(ProductDto model)
        {
            var product = new Product { ProductID = model.ProductID };
            return productService.DeleteProductAsync(product);
        }

        public Task<int> DeleteProductRangeAsync(int index, int length, DataRequest<Product> request)
        {
            return productService.DeleteProductRangeAsync(index, length, request);
        }

        public async Task<ProductDto> GetProductAsync(string id)
        {
            var product = await productService.GetProductAsync(id);
            var model = await DtoAssembler.CreateProductModelAsync(product, includeAllFields: true, null);
            return model;
        }

        public async Task<IList<ProductDto>> GetProductsAsync(int skip, int take, DataRequest<Product> request, Windows.UI.Core.CoreDispatcher dispatcher = null)
        {
            var models = new List<ProductDto>();
            var products = await productService.GetProductsAsync(skip, take, request);
            foreach (var item in products)
            {
                var dto = await DtoAssembler.CreateProductModelAsync(item, includeAllFields: false, dispatcher);
                models.Add(dto);
            }
            return models;
        }

        public Task<int> GetProductsCountAsync(DataRequest<Product> request)
        {
            return productService.GetProductsCountAsync(request);
        }

        public async Task<int> UpdateProductAsync(ProductDto model)
        {
            string id = model.ProductID;
            int rtn = 0;
            Product product = !string.IsNullOrEmpty(id) ? await productService.GetProductAsync(model.ProductID) : new Product();
            if (product != null)
            {
                DtoAssembler.UpdateProductFromModel(product, model);
                rtn = await productService.UpdateProductAsync(product);
                // TODO: verificare l'effetiva utilità nel'aggiornare l'oggetto nodel
                var item = await productService.GetProductAsync(id);
                var newmodel = await DtoAssembler.CreateProductModelAsync(item, includeAllFields: true, null);
                model.Merge(newmodel);
            }
            return rtn;
        }
    }
}
