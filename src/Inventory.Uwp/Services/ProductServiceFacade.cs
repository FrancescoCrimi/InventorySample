using Inventory.Application;
using Inventory.Domain.Model;
using Inventory.Infrastructure.Common;
using Inventory.Uwp.Dto;
using Inventory.Uwp.Tools;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory.Uwp.Services
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
            var product = new Product { Id = model.Id };
            return productService.DeleteProductAsync(product);
        }

        public Task<int> DeleteProductRangeAsync(int index, int length, DataRequest<Product> request)
        {
            return productService.DeleteProductRangeAsync(index, length, request);
        }

        public async Task<ProductDto> GetProductAsync(long id)
        {
            var product = await productService.GetProductAsync(id);
            var model = DtoAssembler.DtoFromProduct(product, includeAllFields: true);
            return model;
        }

        public async Task<List<ProductDto>> GetProductsAsync(int skip, int take, DataRequest<Product> request, Windows.UI.Core.CoreDispatcher dispatcher = null)
        {
            var models = new List<ProductDto>();
            var products = await productService.GetProductsAsync(skip, take, request);
            foreach (var item in products)
            {
                var dto = DtoAssembler.DtoFromProduct(item, includeAllFields: false);
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
            long id = model.Id;
            int rtn = 0;
            Product product = id > 0 ? await productService.GetProductAsync(model.Id) : new Product();
            if (product != null)
            {
                DtoAssembler.UpdateProductFromDto(product, model);
                rtn = await productService.UpdateProductAsync(product);
                // TODO: verificare l'effetiva utilità nel'aggiornare l'oggetto nodel
                var item = await productService.GetProductAsync(id);
                var newmodel = DtoAssembler.DtoFromProduct(item, includeAllFields: true);
                model.Merge(newmodel);
            }
            return rtn;
        }
    }
}
