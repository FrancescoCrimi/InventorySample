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

using Inventory.Models;
using Inventory.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Inventory.ViewModels
{
    public class OrdersViewModel : ObservableRecipient //ViewModelBase
    {
        private readonly ILogger<OrdersViewModel> logger;
        private readonly IMessageService messageService;
        private readonly IContextService contextService;
        private readonly IOrderService orderService;

        public OrdersViewModel(ILogger<OrdersViewModel> logger,
                               IMessageService messageService,
                               IContextService contextService,
                               IOrderService orderService,
                               OrderListViewModel orderListViewModel,
                               OrderDetailsViewModel orderDetailsViewModel,
                               OrderItemListViewModel orderItemListViewModel)
        {
            this.logger = logger;
            this.messageService = messageService;
            this.contextService = contextService;
            this.orderService = orderService;
            OrderList = orderListViewModel;
            OrderDetails = orderDetailsViewModel;
            OrderItemList = orderItemListViewModel;
        }

        public bool IsMainView => contextService.IsMainView;






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
            messageService.Subscribe<OrderListViewModel>(this, OnMessage);
            OrderList.Subscribe();
            OrderDetails.Subscribe();
            OrderItemList.Subscribe();
        }
        public void Unsubscribe()
        {
            messageService.Unsubscribe(this);
            OrderList.Unsubscribe();
            OrderDetails.Unsubscribe();
            OrderItemList.Unsubscribe();
        }

        private async void OnMessage(OrderListViewModel viewModel, string message, object args)
        {
            if (viewModel == OrderList && message == "ItemSelected")
            {
                await contextService.RunAsync(() =>
                {
                    OnItemSelected();
                });
            }
        }

        private async void OnItemSelected()
        {
            if (OrderDetails.IsEditMode)
            {
                //StatusReady();
                messageService.Send(this, "StatusMessage", "Ready");
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

        private async Task PopulateDetails(OrderModel selected)
        {
            try
            {
                var model = await orderService.GetOrderAsync(selected.OrderID);
                selected.Merge(model);
            }
            catch (Exception ex)
            {
                //LogException("Orders", "Load Details", ex);
                logger.LogCritical(ex, "Load Details");
            }
        }

        private async Task PopulateOrderItems(OrderModel selectedItem)
        {
            try
            {
                if (selectedItem != null)
                {
                    await OrderItemList.LoadAsync(new OrderItemListArgs { OrderID = selectedItem.OrderID }, silent: true);
                }
            }
            catch (Exception ex)
            {
                //LogException("Orders", "Load OrderItems", ex);
                logger.LogCritical(ex, "Load OrderItems");
            }
        }
    }
}
