using CiccioSoft.Inventory.Application;
using CiccioSoft.Inventory.Application.Impl;
using CiccioSoft.Inventory.Domain.Model;
using CiccioSoft.Inventory.Infrastructure.Common;
using Inventory.UwpApp.Dto;
using Inventory.UwpApp.Library.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inventory.UwpApp.Services
{
    public class ProductCollection : VirtualRangeCollection<ProductDto>
    {
        private readonly ProductServiceFacade productService;

        public ProductCollection(ProductServiceFacade productService)
        {
            this.productService = productService;
        }

        protected override ProductDto CreateDummyEntity()
        {
            return new ProductDto() {  Name = "Dummy Product" };
        }

        protected override async Task<int> GetCountAsync()
        {
            int result = await productService.GetProductsCountAsync(new DataRequest<Product>());
            return result;
        }

        protected override async Task<IList<ProductDto>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken)
        {
            //Todo: fix cancellationToken
            var result = await productService.GetProductsAsync(skip, take, new DataRequest<Product>(), dispatcher);
            return result;
        }
    }
}
