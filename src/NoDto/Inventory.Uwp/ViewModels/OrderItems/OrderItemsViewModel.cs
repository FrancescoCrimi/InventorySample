// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.

using CommunityToolkit.Mvvm.Messaging;
using Inventory.Domain.Aggregates.OrderAggregate;
using Inventory.Infrastructure.Logging;
using Inventory.Uwp.ViewModels.Common;
using Inventory.Uwp.ViewModels.Message;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Inventory.Uwp.ViewModels.OrderItems
{
    public class OrderItemsViewModel : ViewModelBase
    {
        private readonly ILogger _logger;

        public OrderItemsViewModel(ILogger<OrderItemsViewModel> logger,
                                   OrderItemListViewModel orderItemListViewModel,
                                   OrderItemDetailsViewModel orderItemDetailsViewModel)
            : base()
        {
            _logger = logger;
            OrderItemList = orderItemListViewModel;
            OrderItemDetails = orderItemDetailsViewModel;
        }

        public OrderItemListViewModel OrderItemList { get; set; }

        public OrderItemDetailsViewModel OrderItemDetails { get; set; }

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
            Messenger.Register<ViewModelsMessage<OrderItem>>(this, OnMessage);
            OrderItemList.Subscribe();
            OrderItemDetails.Subscribe();
        }

        public void Unsubscribe()
        {
            Messenger.UnregisterAll(this);
            OrderItemList.Unsubscribe();
            OrderItemDetails.Unsubscribe();
        }

        private void OnMessage(object recipient, ViewModelsMessage<OrderItem> message)
        {
            if (message.Id != 0 && message.Value == "ItemSelected")
            {
                OnItemSelected();
            }
        }

        private async void OnItemSelected()
        {
            if (OrderItemDetails.IsEditMode)
            {
                StatusReady();
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
        }

        private async Task PopulateDetails(OrderItem selected)
        {
            try
            {
                await OrderItemDetails.LoadAsync(new OrderItemDetailsArgs { OrderID = selected.OrderId, OrderLine = selected.OrderLine });
            }
            catch (Exception ex)
            {
                _logger.LogError(LogEvents.LoadDetails, ex, "Load Details");
            }
        }
    }
}
