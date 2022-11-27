using Inventory.Domain.Model;
using Inventory.Infrastructure.Common;
using Inventory.Uwp.Dto;
using Inventory.Uwp.Services;
using Inventory.Uwp.Library.Common;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inventory.Uwp.Services.VirtualCollections
{
    public class ProductCollection : VirtualRangeCollection<ProductDto>
    {
        private readonly ProductServiceFacade productService;
        private string searchString = string.Empty;

        public ProductCollection(ProductServiceFacade productService)
        {
            this.productService = productService;
        }

        public async override Task LoadAsync(string searchString = "")
        {
            this.searchString = searchString;
            await LoadAsync();
        }

        protected override ProductDto CreateDummyEntity()
        {
            return new ProductDto() { Name = "Dummy Product" };
        }

        protected override async Task<int> GetCountAsync()
        {
            int result = await productService.GetProductsCountAsync(new DataRequest<Product>());
            return result;
        }

        protected override async Task<List<ProductDto>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken)
        {
            //Todo: fix cancellationToken
            var result = await productService.GetProductsAsync(skip, take, new DataRequest<Product>(), dispatcher);
            return result;
        }
    }
}
