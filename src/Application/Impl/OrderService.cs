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
using Inventory.Domain.Repository;
using Inventory.Infrastructure.Common;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.Application.Impl
{
    public class OrderService : IOrderService
    {
        private readonly IServiceProvider serviceProvider;

        public OrderService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task<Order> GetOrderAsync(long id)
        {
            using (var dataService = serviceProvider.GetService<IOrderRepository>())
            {
                var item = await dataService.GetOrderAsync(id);
                return item;
            }
        }

        public async Task<IList<Order>> GetOrdersAsync(int skip, int take, DataRequest<Order> request)
        {
            using (var dataService = serviceProvider.GetService<IOrderRepository>())
            {
                var models = await dataService.GetOrdersAsync(skip, take, request);
                return models;
            }
        }

        public async Task<int> GetOrdersCountAsync(DataRequest<Order> request)
        {
            using (var dataService = serviceProvider.GetService<IOrderRepository>())
            {
                return await dataService.GetOrdersCountAsync(request);
            }
        }

        public async Task<Order> CreateNewOrderAsync(long customerID)
        {
            var order = new Order
            {
                CustomerID = customerID,
                OrderDate = DateTime.UtcNow,
                Status = 0
            };

            if (customerID > 0)
            {
                using (var dataService = serviceProvider.GetService<ICustomerRepository>())
                {
                    var customer = await dataService.GetCustomerAsync(customerID);
                    if (customer != null)
                    {
                        order.CustomerID = customerID;
                        order.ShipAddress = customer.AddressLine1;
                        order.ShipCity = customer.City;
                        order.ShipRegion = customer.Region;
                        order.ShipCountryCode = customer.CountryCode;
                        order.ShipPostalCode = customer.PostalCode;
                        //order.Customer = CustomerService.CreateCustomerModelAsync(customer, includeAllFields: true);
                        order.Customer = customer;
                    }
                }
            }
            return order;
        }

        public async Task<int> UpdateOrderAsync(Order order)
        {
            using (var dataService = serviceProvider.GetService<IOrderRepository>())
            {
                return await dataService.UpdateOrderAsync(order);
            }
        }

        public async Task<int> DeleteOrderAsync(Order order)
        {
            using (var dataService = serviceProvider.GetService<IOrderRepository>())
            {
                return await dataService.DeleteOrdersAsync(order);
            }
        }

        public async Task<int> DeleteOrderRangeAsync(int index, int length, DataRequest<Order> request)
        {
            using (var dataService = serviceProvider.GetService<IOrderRepository>())
            {
                var items = await dataService.GetOrderKeysAsync(index, length, request);
                return await dataService.DeleteOrdersAsync(items.ToArray());
            }
        }
    }
}
