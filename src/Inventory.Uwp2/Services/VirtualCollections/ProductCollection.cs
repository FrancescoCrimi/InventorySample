#region copyright
// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************
#endregion

using Inventory.Domain.Model;
using Inventory.Infrastructure.Common;
using Inventory.Uwp.Dto;
using Inventory.Uwp.Library.Common;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inventory.Uwp.Services.VirtualCollections
{
    public class ProductCollection : VirtualRangeCollection<ProductDto>
    {
        private readonly ProductServiceFacade productService;
        private DataRequest<Product> request;

        public ProductCollection(ProductServiceFacade productService)
        {
            this.productService = productService;
        }

        public async  Task LoadAsync(DataRequest<Product> request)
        {
            this.request = request;
            await LoadAsync();
        }

        protected override ProductDto CreateDummyEntity()
        {
            return new ProductDto() { Name = "Dummy Product" };
        }

        protected async override Task<int> GetCountAsync()
        {
            int result = await productService.GetProductsCountAsync(request);
            return result;
        }

        protected async override Task<IList<ProductDto>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken)
        {
            //Todo: fix cancellationToken
            var result = await productService.GetProductsAsync(skip, take, request, dispatcher);
            return result;
        }
    }
}
