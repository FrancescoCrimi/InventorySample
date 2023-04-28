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
using Inventory.Infrastructure.Logging;
using Inventory.Uwp.Dto;
using Inventory.Uwp.Library.Common;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inventory.Uwp.Services.VirtualCollections
{
    public class ProductCollection : VirtualRangeCollection<ProductDto>
    {
        private readonly ILogger _logger;
        private readonly ProductService _productService;
        private DataRequest<Product> _request;

        public ProductCollection(ILogger<ProductCollection> logger,
                                 ProductService productService)
            : base(logger)
        {
            _logger = logger;
            _productService = productService;
        }

        public async Task LoadAsync(DataRequest<Product> request)
        {
            _request = request;
            await LoadAsync();
        }

        protected override ProductDto CreateDummyEntity()
        {
            return new ProductDto() { Name = "Dummy Product" };
        }

        protected override async Task<int> GetCountAsync()
        {
            int result = await _productService.GetProductsCountAsync(_request);
            return result;
        }

        protected override async Task<IList<ProductDto>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken)
        {
            try
            {
                //Todo: fix cancellationToken
                var result = await _productService.GetProductsAsync(skip, take, _request, dispatcher);
                return result;
            }
            catch (System.Exception ex)
            {
                //LogException("ProductCollection", "Fetch", ex);
                _logger.LogError(LogEvents.Fetch, ex, "Load Product Error");
            }
            return null;
        }
    }
}
