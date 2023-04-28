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

using CommunityToolkit.Mvvm.Messaging;
using Inventory.Infrastructure.Logging;
using Inventory.Uwp.Dto;
using Inventory.Uwp.Services;
using Inventory.Uwp.ViewModels.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Inventory.Uwp.ViewModels.OrderItems
{
    public class OrderItemsViewModel : ViewModelBase
    {
        private readonly ILogger _logger;
        private readonly OrderItemService _orderItemService;

        public OrderItemsViewModel(ILogger<OrderItemsViewModel> logger,
                                   OrderItemService orderItemService,
                                   OrderItemListViewModel orderItemListViewModel,
                                   OrderItemDetailsViewModel orderItemDetailsViewModel)
            : base()
        {
            _logger = logger;
            _orderItemService = orderItemService;
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
            //MessageService.Subscribe<OrderItemListViewModel>(this, OnMessage);
            Messenger.Register<ItemMessage<OrderItemDto>>(this, OnOrderItemMessage);
            OrderItemList.Subscribe();
            OrderItemDetails.Subscribe();
        }

        public void Unsubscribe()
        {
            //MessageService.Unsubscribe(this);
            Messenger.UnregisterAll(this);
            OrderItemList.Unsubscribe();
            OrderItemDetails.Unsubscribe();
        }

        private void OnOrderItemMessage(object recipient, ItemMessage<OrderItemDto> message)
        {
            //    throw new NotImplementedException();
            //}
            //private async void OnMessage(OrderItemListViewModel viewModel, string message, object args)
            //{
            if (recipient == OrderItemList && message.Message == "ItemSelected")
            {
                //await ContextService.RunAsync(() =>
                //{
                OnItemSelected();
                //});
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
            OrderItemDetails.Item = selected;
        }

        private async Task PopulateDetails(OrderItemDto selected)
        {
            try
            {
                var model = await _orderItemService.GetOrderItemAsync(selected.OrderId, selected.OrderLine);
                selected.Merge(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(LogEvents.LoadDetails, ex, "Load Details");
            }
        }
    }
}
