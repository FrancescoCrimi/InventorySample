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

using Inventory.Application;
using Inventory.Domain.Model;
using Inventory.Infrastructure.Common;
using Inventory.Uwp.Library.Common;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inventory.Uwp.Services.VirtualCollections
{
    public class OrderCollection : VirtualRangeCollection<Order>
    {
        private readonly OrderService _orderService;
        private DataRequest<Order> _request;

        public OrderCollection(OrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task LoadAsync(DataRequest<Order> request)
        {
            this._request = request;
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
            //Todo: fix cancellationToken
            var result = await _orderService.GetOrdersAsync(skip, take, _request);
            return result;
        }
    }
}
