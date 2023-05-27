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

using Inventory.Application;
using Inventory.Domain.Aggregates.OrderAggregate;
using Inventory.Infrastructure.Common;
using Inventory.Infrastructure.Logging;
using Inventory.Uwp.Library.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inventory.Uwp.Services.VirtualCollections
{
    public class OrderCollection : VirtualRangeCollection<Order>
    {
        private readonly ILogger _logger;
        private readonly OrderService _orderService;
        private DataRequest<Order> _request;

        public OrderCollection(ILogger<OrderCollection> logger,
                               OrderService orderService)
            : base(logger)
        {
            _logger = logger;
            _orderService = orderService;
        }

        // TODO: fix here request
        public async Task LoadAsync(DataRequest<Order> request)
        {
            _request = request;
            await LoadAsync();
        }

        protected override Order CreateDummyEntity()
        {
            return new Order() { };
        }

        protected async override Task<int> GetCountAsync()
        {
            var result = await _orderService.GetOrdersCountAsync(_request);
            return result;
        }

        protected async override Task<IList<Order>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken)
        {
            try
            {
                //Todo: fix cancellationToken
                var result = await _orderService.GetOrdersAsync(skip, take, _request);
                return result;
            }
            catch (Exception ex)
            {
                //LogException("OrderCollection", "Fetch", ex);
                _logger.LogError(LogEvents.Fetch, ex, "Load Order Error");
            }
            return null;
        }
    }
}
