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
using Inventory.Interface.Dto;
using Inventory.Interface;
using Inventory.Uwp.Library.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inventory.Uwp.Services.VirtualCollections
{
    public class OrderCollection : VirtualRangeCollection<OrderDto>
    {
        private readonly ILogger _logger;
        private readonly IOrderServiceFacade _orderService;
        private DataRequest<Order> _request;

        public OrderCollection(ILogger<OrderCollection> logger,
                               IOrderServiceFacade orderService)
            : base(logger)
        {
            _logger = logger;
            _orderService = orderService;
        }

        public async Task LoadAsync(DataRequest<Order> request)
        {
            _request = request;
            await LoadAsync();
        }

        protected override OrderDto CreateDummyEntity()
        {
            return new OrderDto() { };
        }

        protected override async Task<int> GetCountAsync()
        {
            int result = await _orderService.GetOrdersCountAsync(_request);
            return result;
        }

        protected override async Task<IList<OrderDto>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken)
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
