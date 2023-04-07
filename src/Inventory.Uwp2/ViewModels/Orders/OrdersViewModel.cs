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

using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using Inventory.Application;
using Inventory.Domain.Model;
using Inventory.Uwp.ViewModels.Common;
using Inventory.Uwp.ViewModels.Message;
using Inventory.Uwp.ViewModels.OrderItems;
using Microsoft.Extensions.Logging;

namespace Inventory.Uwp.ViewModels.Orders
{
    public class OrdersViewModel : ViewModelBase
    {
        private readonly ILogger<OrdersViewModel> _logger;
        private readonly IOrderService _orderService;

        public OrdersViewModel(ILogger<OrdersViewModel> logger,
                               IOrderService orderService,
                               OrderListViewModel orderListViewModel,
                               OrderDetailsViewModel orderDetailsViewModel,
                               OrderItemListViewModel orderItemListViewModel)
            : base()
        {
            _logger = logger;
            _orderService = orderService;
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
            //MessageService.Subscribe<OrderListViewModel>(this, OnMessage);
            Messenger.Register<OrderChangedMessage>(this, OnMessage);
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

        private async void OnMessage(object recipient, OrderChangedMessage message)
        {
            if (message.Value == "ItemSelected")
            {
                if (message.Id != 0)
                {
                    //TODO: rendere il metodo OnItemSelected cancellabile
                    await OnItemSelected();
                }
            }
        }

        //private async void OnMessage(OrderListViewModel viewModel, string message, object args)
        //{
        //    if (viewModel == OrderList && message == "ItemSelected")
        //    {
        //        await ContextService.RunAsync(() =>
        //        {
        //            OnItemSelected();
        //        });
        //    }
        //}

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

        private async Task PopulateDetails(Order selected)
        {
            try
            {
                var model = await _orderService.GetOrderAsync(selected.Id);
                selected.Merge(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Load Details");
            }
        }

        private async Task PopulateOrderItems(Order selectedItem)
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
                _logger.LogError(ex, "Load OrderItems");
            }
        }
    }
}
