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
    public class OrderItemsViewModel : ObservableRecipient // ViewModelBase
    {
        private readonly ILogger<OrderItemsViewModel> logger;
        private readonly IMessageService messageService;
        private readonly IContextService contextService;
        private readonly IOrderItemService orderItemService;

        public OrderItemsViewModel(ILogger<OrderItemsViewModel> logger,
                                   IMessageService messageService,
                                   IContextService contextService,
                                   IOrderItemService orderItemService,
                                   OrderItemListViewModel orderItemListViewModel,
                                   OrderItemDetailsViewModel orderItemDetailsViewModel)
        {
            this.logger = logger;
            this.messageService = messageService;
            this.contextService = contextService;
            this.orderItemService = orderItemService;
            OrderItemList = orderItemListViewModel;
            OrderItemDetails = orderItemDetailsViewModel;
        }

        public OrderItemListViewModel OrderItemList { get; set; }
        public OrderItemDetailsViewModel OrderItemDetails { get; set; }

        public bool IsMainView => contextService.IsMainView;

        public async Task LoadAsync(OrderItemListArgs args)
        {
            await OrderItemList.LoadAsync(args);
        }
        public void Unload()
        {
            OrderItemDetails.CancelEdit();
            OrderItemList.Unload();
        }

        public void Subscribe()
        {
            messageService.Subscribe<OrderItemListViewModel>(this, OnMessage);
            OrderItemList.Subscribe();
            OrderItemDetails.Subscribe();
        }
        public void Unsubscribe()
        {
            messageService.Unsubscribe(this);
            OrderItemList.Unsubscribe();
            OrderItemDetails.Unsubscribe();
        }

        private async void OnMessage(OrderItemListViewModel viewModel, string message, object args)
        {
            if (viewModel == OrderItemList && message == "ItemSelected")
            {
                await contextService.RunAsync(() =>
                {
                    OnItemSelected();
                });
            }
        }

        private async void OnItemSelected()
        {
            if (OrderItemDetails.IsEditMode)
            {
                //StatusReady();
                messageService.Send(this, "StatusMessage", "Ready");
                OrderItemDetails.CancelEdit();
            }
            var selected = OrderItemList.SelectedItem;
            if (!OrderItemList.IsMultipleSelection)
            {
                if (selected != null && !selected.IsEmpty)
                {
                    await PopulateDetails(selected);
                }
            }
            OrderItemDetails.Item = selected;
        }

        private async Task PopulateDetails(OrderItemModel selected)
        {
            try
            {
                var model = await orderItemService.GetOrderItemAsync(selected.OrderID, selected.OrderLine);
                selected.Merge(model);
            }
            catch (Exception ex)
            {
                //LogException("OrderItems", "Load Details", ex);
                logger.LogCritical(ex, "Load Details");
            }
        }
    }
}
