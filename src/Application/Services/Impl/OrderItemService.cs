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

using CiccioSoft.Inventory.Application.Models;
using CiccioSoft.Inventory.Domain.Model;
using CiccioSoft.Inventory.Domain.Repository;
using CiccioSoft.Inventory.Infrastructure.Common;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CiccioSoft.Inventory.Application.Services.Impl
{
    public class OrderItemService : IOrderItemService
    {
        private readonly IServiceProvider serviceProvider;

        public OrderItemService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task<OrderItemModel> GetOrderItemAsync(long orderID, int lineID)
        {
            using (var dataService = serviceProvider.GetService<IOrderItemRepository>())
            {
                return await GetOrderItemAsync(dataService, orderID, lineID);
            }
        }

        public Task<IList<OrderItemModel>> GetOrderItemsAsync(DataRequest<OrderItem> request)
        {
            // OrderItems are not virtualized
            return GetOrderItemsAsync(0, 100, request);
        }

        public async Task<IList<OrderItemModel>> GetOrderItemsAsync(int skip, int take, DataRequest<OrderItem> request)
        {
            var models = new List<OrderItemModel>();
            using (var dataService = serviceProvider.GetService<IOrderItemRepository>())
            {
                var items = await dataService.GetOrderItemsAsync(skip, take, request);
                foreach (var item in items)
                {
                    models.Add(CreateOrderItemModelAsync(item, includeAllFields: false));
                }
                return models;
            }
        }

        public async Task<int> GetOrderItemsCountAsync(DataRequest<OrderItem> request)
        {
            using (var dataService = serviceProvider.GetService<IOrderItemRepository>())
            {
                return await dataService.GetOrderItemsCountAsync(request);
            }
        }

        public async Task<int> UpdateOrderItemAsync(OrderItemModel model)
        {
            using (var dataService = serviceProvider.GetService<IOrderItemRepository>())
            {
                var orderItem = model.OrderLine > 0 ? await dataService.GetOrderItemAsync(model.OrderID, model.OrderLine) : new OrderItem();
                if (orderItem != null)
                {
                    UpdateOrderItemFromModel(orderItem, model);
                    await dataService.UpdateOrderItemAsync(orderItem);
                    model.Merge(await GetOrderItemAsync(dataService, orderItem.OrderID, orderItem.OrderLine));
                }
                return 0;
            }
        }

        public async Task<int> DeleteOrderItemAsync(OrderItemModel model)
        {
            var orderItem = new OrderItem { OrderID = model.OrderID, OrderLine = model.OrderLine };
            using (var dataService = serviceProvider.GetService<IOrderItemRepository>())
            {
                return await dataService.DeleteOrderItemsAsync(orderItem);
            }
        }

        public async Task<int> DeleteOrderItemRangeAsync(int index, int length, DataRequest<OrderItem> request)
        {
            using (var dataService = serviceProvider.GetService<IOrderItemRepository>())
            {
                var items = await dataService.GetOrderItemKeysAsync(index, length, request);
                return await dataService.DeleteOrderItemsAsync(items.ToArray());
            }
        }


        static private async Task<OrderItemModel> GetOrderItemAsync(IOrderItemRepository dataService, long orderID, int lineID)
        {
            var item = await dataService.GetOrderItemAsync(orderID, lineID);
            if (item != null)
            {
                return CreateOrderItemModelAsync(item, includeAllFields: true);
            }
            return null;
        }


        static public OrderItemModel CreateOrderItemModelAsync(OrderItem source, bool includeAllFields)
        {
            var model = new OrderItemModel()
            {
                OrderID = source.OrderID,
                OrderLine = source.OrderLine,
                ProductID = source.ProductID,
                Quantity = source.Quantity,
                UnitPrice = source.UnitPrice,
                Discount = source.Discount,
                TaxType = source.TaxType,
                Product = ProductService.CreateProductModelAsync(source.Product, includeAllFields)
            };
            return model;
        }

        private void UpdateOrderItemFromModel(OrderItem target, OrderItemModel source)
        {
            target.OrderID = source.OrderID;
            target.OrderLine = source.OrderLine;
            target.ProductID = source.ProductID;
            target.Quantity = source.Quantity;
            target.UnitPrice = source.UnitPrice;
            target.Discount = source.Discount;
            target.TaxType = source.TaxType;
        }
    }
}
