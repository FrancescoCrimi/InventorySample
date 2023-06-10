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

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Inventory.Domain.Model;
using Inventory.Infrastructure.Common;
using Inventory.Infrastructure.Logging;
using Inventory.Uwp.Dto;
using Inventory.Uwp.Library.Common;
using Inventory.Uwp.Services;
using Inventory.Uwp.ViewModels.Common;
using Inventory.Uwp.ViewModels.Message;
using Inventory.Uwp.Views.OrderItems;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Inventory.Uwp.ViewModels.OrderItems
{
    public class OrderItemListViewModel : GenericListViewModel<OrderItemDto>
    {
        private readonly ILogger _logger;
        private readonly OrderItemService _orderItemService;
        private readonly NavigationService _navigationService;
        private readonly WindowManagerService _windowService;

        public OrderItemListViewModel(ILogger<OrderItemListViewModel> logger,
                                      OrderItemService orderItemService,
                                      NavigationService navigationService,
                                      WindowManagerService windowService)
            : base()
        {
            _logger = logger;
            _orderItemService = orderItemService;
            _navigationService = navigationService;
            _windowService = windowService;
        }

        public OrderItemListArgs ViewModelArgs { get; private set; }

        public async Task LoadAsync(OrderItemListArgs args, bool silent = false)
        {
            ViewModelArgs = args ?? new OrderItemListArgs();
            Query = ViewModelArgs.Query;

            if (silent)
            {
                await RefreshAsync();
            }
            else
            {
                StartStatusMessage("Loading order items...");
                if (await RefreshAsync())
                {
                    EndStatusMessage("OrderItems loaded");
                }
            }
        }
        public void Unload()
        {
            ViewModelArgs.Query = Query;
        }

        public void Subscribe()
        {
            Messenger.Register<ViewModelsMessage<OrderItemDto>>(this, OnMessage);
        }

        public void Unsubscribe()
        {
            Messenger.RegisterAll(this);
        }

        public OrderItemListArgs CreateArgs()
        {
            return new OrderItemListArgs
            {
                Query = Query,
                OrderBy = ViewModelArgs.OrderBy,
                OrderByDesc = ViewModelArgs.OrderByDesc,
                OrderID = ViewModelArgs.OrderID
            };
        }

        public async Task<bool> RefreshAsync()
        {
            bool isOk = true;
            Items = null;
            ItemsCount = 0;
            SelectedItem = null;

            //todo: questa try é forse inutile, verificare virtualcollection loadasync
            try
            {
                DataRequest<OrderItem> request = BuildDataRequest();
                Items = await _orderItemService.GetOrderItemsAsync(request);
            }
            catch (Exception ex)
            {
                Items = new List<OrderItemDto>();
                StatusError($"Error loading Order items: {ex.Message}");
                _logger.LogError(LogEvents.Refresh, ex, "Error loading Order items");
                isOk = false;
            }

            ItemsCount = Items.Count;
            OnPropertyChanged(nameof(Title));
            return isOk;
        }

        public ICommand OpenInNewViewCommand => new RelayCommand(OnOpenInNewView);
        private async void OnOpenInNewView()
        {
            if (SelectedItem != null)
            {
                await _windowService.OpenInNewWindow<OrderItemPage>(new OrderItemDetailsArgs { OrderID = SelectedItem.OrderId, OrderLine = SelectedItem.OrderLine });
            }
        }

        protected override async void OnNew()
        {
            if (IsMainView)
            {
                await _windowService.OpenInNewWindow<OrderItemPage>(new OrderItemDetailsArgs { OrderID = ViewModelArgs.OrderID });
            }
            else
            {
                _navigationService.Navigate<OrderItemPage>(new OrderItemDetailsArgs { OrderID = ViewModelArgs.OrderID });
            }

            StatusReady();
        }

        protected override async void OnRefresh()
        {
            StartStatusMessage("Loading order items...");
            if (await RefreshAsync())
            {
                EndStatusMessage("Order items loaded");
            }
        }

        protected override async void OnDeleteSelection()
        {
            StatusReady();
            if (await ShowDialogAsync("Confirm Delete", "Are you sure you want to delete selected order items?", "Ok", "Cancel"))
            {
                int count = 0;
                try
                {
                    if (SelectedIndexRanges != null)
                    {
                        count = SelectedIndexRanges.Sum(r => r.Length);
                        StartStatusMessage($"Deleting {count} order items...");
                        await DeleteRangesAsync(SelectedIndexRanges);
                        Messenger.Send(new ViewModelsMessage<OrderItemDto>("ItemRangesDeleted", SelectedIndexRanges));
                    }
                    else if (SelectedItems != null)
                    {
                        count = SelectedItems.Count();
                        StartStatusMessage($"Deleting {count} order items...");
                        await DeleteItemsAsync(SelectedItems);
                        Messenger.Send(new ViewModelsMessage<OrderItemDto>("ItemsDeleted", SelectedItems));
                    }
                }
                catch (Exception ex)
                {
                    StatusError($"Error deleting {count} order items: {ex.Message}");
                    _logger.LogError(LogEvents.Delete, ex, $"Error deleting {count} order items");
                    count = 0;
                }
                await RefreshAsync();
                SelectedIndexRanges = null;
                SelectedItems = null;
                if (count > 0)
                {
                    EndStatusMessage($"{count} order items deleted");
                }
            }
        }

        private async Task DeleteItemsAsync(IEnumerable<OrderItemDto> models)
        {
            foreach (var model in models)
            {
                await _orderItemService.DeleteOrderItemAsync(model);
            }
        }

        private async Task DeleteRangesAsync(IEnumerable<IndexRange> ranges)
        {
            DataRequest<OrderItem> request = BuildDataRequest();
            foreach (var range in ranges)
            {
                await _orderItemService.DeleteOrderItemRangeAsync(range.Index, range.Length, request);
            }
        }

        private DataRequest<OrderItem> BuildDataRequest()
        {
            var request = new DataRequest<OrderItem>()
            {
                Query = Query,
                OrderBy = ViewModelArgs.OrderBy,
                OrderByDesc = ViewModelArgs.OrderByDesc
            };
            if (ViewModelArgs.OrderID > 0)
            {
                request.Where = (r) => r.OrderId == ViewModelArgs.OrderID;
            }
            return request;
        }

        private async void OnMessage(object recipient, ViewModelsMessage<OrderItemDto> message)
        {
            switch (message.Value)
            {
                case "NewItemSaved":
                case "ItemDeleted":
                case "ItemsDeleted":
                case "ItemRangesDeleted":
                    await RefreshAsync();
                    break;
            }
        }
    }
}
