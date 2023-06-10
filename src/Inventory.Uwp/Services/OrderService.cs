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
    public class OrderService
    {
        private readonly ILogger<OrderService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public OrderService(ILogger<OrderService> logger,
                                  IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task<OrderDto> CreateNewOrderAsync(long customerID)
        {
            using (var dataService = _serviceProvider.GetService<ICustomerRepository>())
            {
                var customer = await dataService.GetCustomerAsync(customerID);
                Order order = Order.CreateNewOrder(customer);
                OrderDto model = DtoAssembler.DtoFromOrder(order);
                if (customerID > 0)
                    model.Customer = DtoAssembler.DtoFromCustomer(order.Customer);
                return model;
            }
        }

        public async Task<int> DeleteOrderAsync(OrderDto model)
        {
            using (var orderRepository = _serviceProvider.GetService<IOrderRepository>())
            {
                var order = await orderRepository.GetOrderAsync(model.Id);
                return await orderRepository.DeleteOrdersAsync(order);
            }
        }

        public async Task<int> DeleteOrderRangeAsync(int index, int length, DataRequest<Order> request)
        {
            using (var orderRepository = _serviceProvider.GetService<IOrderRepository>())
            {
                var items = await orderRepository.GetOrderKeysAsync(index, length, request);
                return await orderRepository.DeleteOrdersAsync(items.ToArray());
            }
        }

        public async Task<OrderDto> GetOrderAsync(long id)
        {
            using (var orderRepository = _serviceProvider.GetService<IOrderRepository>())
            {
                var item = await orderRepository.GetOrderAsync(id);
                OrderDto model = DtoAssembler.DtoFromOrder(item);
                if (item.Customer != null)
                    model.Customer = DtoAssembler.DtoFromCustomer(item.Customer);
                return model;
            }
        }

        public async Task<List<OrderDto>> GetOrdersAsync(int skip,
                                                         int take,
                                                         DataRequest<Order> request,
                                                         Windows.UI.Core.CoreDispatcher dispatcher = null)
        {
            using (var orderRepository = _serviceProvider.GetService<IOrderRepository>())
            {
                var models = new List<OrderDto>();
                var orders = await orderRepository.GetOrdersAsync(skip, take, request);
                foreach (var item in orders)
                {
                    OrderDto dto = DtoAssembler.DtoFromOrder(item);
                    if (item.Customer != null)
                        dto.Customer = DtoAssembler.DtoFromCustomer(item.Customer);
                    models.Add(dto);
                }
                return models;
            }
        }

        public async Task<int> GetOrdersCountAsync(DataRequest<Order> request)
        {
            using (var orderRepository = _serviceProvider.GetService<IOrderRepository>())
            {
                return await orderRepository.GetOrdersCountAsync(request);
            }
        }

        public async Task<int> UpdateOrderAsync(OrderDto model)
        {
            using (var orderRepository = _serviceProvider.GetService<IOrderRepository>())
            {
                int ret = 0;
                long id = model.Id;
                var order = id > 0 ? await orderRepository.GetOrderAsync(model.Id) : new Order();
                if (order != null)
                {
                    DtoAssembler.UpdateOrderFromDto(order, model);
                    ret = await orderRepository.UpdateOrderAsync(order);
                    var item = await orderRepository.GetOrderAsync(id);
                    var newmodel = DtoAssembler.DtoFromOrder(item);
                    model.Merge(newmodel);
                }
                return ret;
            }

        }
    }
}
