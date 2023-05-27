// Copyright (c) Microsoft. All rights reserved.
// Copyright (c) 2023 Francesco Crimi francrim@gmail.com
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inventory.Application;
using Inventory.Domain.Aggregates.ProductAggregate;
using Inventory.Infrastructure.Common;
using Inventory.Infrastructure.Logging;
using Inventory.Uwp.Library.Common;
using Microsoft.Extensions.Logging;

namespace Inventory.Uwp.Services.VirtualCollections
{
    public class ProductCollection : VirtualRangeCollection<Product>
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

        // TODO: fix here request
        public async Task LoadAsync(DataRequest<Product> request)
        {
            _request = request;
            await LoadAsync();
        }

        protected override Product CreateDummyEntity()
        {
            return new Product() { Name = "Dummy Product" };
        }

        protected async override Task<int> GetCountAsync()
        {
            var result = await _productService.GetProductsCountAsync(_request);
            return result;
        }

        protected async override Task<IList<Product>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken)
        {
            try
            {
                //Todo: fix cancellationToken
                var result = await _productService.GetProductsAsync(skip, take, _request);
                return result;
            }
            catch (Exception ex)
            {
                //LogException("ProductCollection", "Fetch", ex);
                _logger.LogError(LogEvents.Fetch, ex, "Load Product Error");
            }
            return null;
        }
    }
}
