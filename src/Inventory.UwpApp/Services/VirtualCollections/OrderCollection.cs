using Inventory.Domain.Model;
using Inventory.Infrastructure.Common;
using Inventory.UwpApp.Dto;
using Inventory.UwpApp.Library.Common;
using System.Collections.Generic;
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
