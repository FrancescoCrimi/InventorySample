﻿#region copyright
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

using CommunityToolkit.Mvvm.Messaging;
using Inventory.Uwp.Dto;
using Inventory.Uwp.Services;
using Inventory.Uwp.ViewModels.Common;
using Inventory.Uwp.ViewModels.OrderItems;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Inventory.Uwp.ViewModels.Orders
{
    public class OrdersViewModel : ViewModelBase
    {
        private readonly ILogger<OrdersViewModel> logger;
        private readonly OrderServiceFacade orderService;

        public OrdersViewModel(ILogger<OrdersViewModel> logger,
                               OrderServiceFacade orderService,
                               OrderListViewModel orderListViewModel,
                               OrderDetailsViewModel orderDetailsViewModel,
                               OrderItemListViewModel orderItemListViewModel)
            : base()
        {
            this.logger = logger;
            this.orderService = orderService;
            OrderList = orderListViewModel;
            OrderDetails = orderDetailsViewModel;
            OrderItemList = orderItemListViewModel;
        }

        public OrderListViewModel OrderList { get; set; }
        public OrderDetailsViewModel OrderDetails { get; set; }
        public OrderItemListViewModel OrderItemList { get; set; }

        public async Task LoadAsync(OrderListArgs args)
        {
            await OrderList.LoadAsync(args);
        }

        public void Unload()
        {
            OrderDetails.CancelEdit();
            OrderList.Unload();
        }

        public void Subscribe()
        {
            Messenger.Register<ItemMessage<OrderDto>>(this, OnOrderMessage);
            OrderList.Subscribe();
            OrderDetails.Subscribe();
            OrderItemList.Subscribe();
        }

        public void Unsubscribe()
        {
            Messenger.UnregisterAll(this);
            OrderList.Unsubscribe();
            OrderDetails.Unsubscribe();
            OrderItemList.Unsubscribe();
        }

        private async void OnOrderMessage(object recipient, ItemMessage<OrderDto> message)
        {
            if (message.Message == "ItemSelected")
            {
                if (message.Value.Id != 0)
                {
                    //TODO: rendere il metodo OnItemSelected cancellabile
                    await OnItemSelected();
                }
            }
        }

        private async Task OnItemSelected()
        {
            if (OrderDetails.IsEditMode)
            {
                StatusReady();
                OrderDetails.CancelEdit();
            }
            OrderItemList.IsMultipleSelection = false;
            var selected = OrderList.SelectedItem;
            if (!OrderList.IsMultipleSelection)
            {
                if (selected != null && !selected.IsEmpty)
                {
                    await PopulateDetails(selected);
                    await PopulateOrderItems(selected);
                }
            }
            OrderDetails.Item = selected;
        }

        private async Task PopulateDetails(OrderDto selected)
        {
            try
            {
                var model = await orderService.GetOrderAsync(selected.Id);
                selected.Merge(model);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Load Details");
            }
        }

        private async Task PopulateOrderItems(OrderDto selectedItem)
        {
            try
            {
                if (selectedItem != null)
                {
                    await OrderItemList.LoadAsync(new OrderItemListArgs { OrderID = selectedItem.Id }, silent: true);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Load OrderItems");
            }
        }
    }
}