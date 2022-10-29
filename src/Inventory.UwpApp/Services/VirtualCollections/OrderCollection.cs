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
    public class OrderCollection : VirtualRangeCollection<OrderDto>
    {
        private readonly OrderServiceFacade orderService;

        public OrderCollection(OrderServiceFacade orderService)
        {
            this.orderService = orderService;
        }

        protected override OrderDto CreateDummyEntity()
        {
            return new OrderDto() { };
        }

        protected override async Task<int> GetCountAsync()
        {
            int result = await orderService.GetOrdersCountAsync(new DataRequest<Order>());
            return result;
        }

        protected override async Task<IList<OrderDto>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken)
        {
            //Todo: fix cancellationToken
            var result = await orderService.GetOrdersAsync(skip, take, new DataRequest<Order>(), dispatcher);
            return result;
        }
    }
}
