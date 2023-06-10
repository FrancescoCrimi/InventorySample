using Inventory.Domain.Model;
using Inventory.Domain.Repository;
using Inventory.Infrastructure.Common;
using Inventory.Uwp.Dto;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.Uwp.Services
{
    public class OrderItemService
    {
        private readonly ILogger<OrderItemService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public OrderItemService(ILogger<OrderItemService> logger,
                                      IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task<int> DeleteOrderItemAsync(OrderItemDto model)
        {
            using (var orderItemRepository = _serviceProvider.GetService<IOrderItemRepository>())
            {
                var orderItem = new OrderItem { OrderId = model.OrderId, OrderLine = model.OrderLine };
                int ret = await orderItemRepository.DeleteOrderItemsAsync(orderItem);
                return ret;
            }
        }

        public async Task<int> DeleteOrderItemRangeAsync(int index, int length, DataRequest<OrderItem> request)
        {
            using (var orderItemRepository = _serviceProvider.GetService<IOrderItemRepository>())
            {
                var items = await orderItemRepository.GetOrderItemKeysAsync(index, length, request);
                return await orderItemRepository.DeleteOrderItemsAsync(items.ToArray());
            }
        }

        public async Task<OrderItemDto> GetOrderItemAsync(long orderID, int lineID)
        {
            using (var orderItemRepository = _serviceProvider.GetService<IOrderItemRepository>())
            {
                var item = await orderItemRepository.GetOrderItemAsync(orderID, lineID);
                var model = DtoAssembler.CreateOrderItemModelAsync(item, includeAllFields: true);
                DtoAssembler.DtoFromProduct(item.Product, true);
                return model;
            }
        }

        public async Task<IList<OrderItemDto>> GetOrderItemsAsync(DataRequest<OrderItem> request)
        {
            using (var orderItemRepository = _serviceProvider.GetService<IOrderItemRepository>())
            {
                var models = new List<OrderItemDto>();
                var items = await orderItemRepository.GetOrderItemsAsync(0, 100, request);
                foreach (var item in items)
                {
                    var model = DtoAssembler.CreateOrderItemModelAsync(item, includeAllFields: false);
                    model.Product = DtoAssembler.DtoFromProduct(item.Product, false);
                    models.Add(model);
                }
                return models;
            }
        }

        public async Task<IList<OrderItemDto>> GetOrderItemsAsync(int skip, int take, DataRequest<OrderItem> request)
        {
            using (var orderItemRepository = _serviceProvider.GetService<IOrderItemRepository>())
            {
                var models = new List<OrderItemDto>();
                var items = await orderItemRepository.GetOrderItemsAsync(skip, take, request);
                foreach (var item in items)
                {
                    models.Add(DtoAssembler.CreateOrderItemModelAsync(item, includeAllFields: false));
                }
                return models;
            }
        }

        public async Task<int> GetOrderItemsCountAsync(DataRequest<OrderItem> request)
        {
            using (var orderItemRepository = _serviceProvider.GetService<IOrderItemRepository>())
            {
                return await orderItemRepository.GetOrderItemsCountAsync(request);
            }
        }

        public async Task<int> UpdateOrderItemAsync(OrderItemDto model)
        {
            using (var orderItemRepository = _serviceProvider.GetService<IOrderItemRepository>())
            {
                var orderItem = model.OrderLine > 0 ? await orderItemRepository.GetOrderItemAsync(model.OrderId, model.OrderLine) : new OrderItem();
                DtoAssembler.UpdateOrderItemFromModel(orderItem, model);
                int suca =  await orderItemRepository.UpdateOrderItemAsync(orderItem);
                var item = await orderItemRepository.GetOrderItemAsync(orderItem.OrderId, orderItem.OrderLine);
                var aaaaa = DtoAssembler.CreateOrderItemModelAsync(item, includeAllFields: true);
                model.Merge(aaaaa);
                return suca;
            }
        }
    }
}
