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
using Inventory.Uwp.Services.VirtualCollections;
using Inventory.Uwp.ViewModels.Common;
using Inventory.Uwp.ViewModels.Message;
using Inventory.Uwp.Views.Orders;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Inventory.Uwp.ViewModels.Orders
{
    public class OrderListViewModel : GenericListViewModel<OrderDto>
    {
        private readonly ILogger _logger;
        private readonly OrderService _orderService;
        private readonly WindowManagerService _windowService;
        private readonly NavigationService _navigationService;
        private readonly OrderCollection _collection;

        public OrderListViewModel(ILogger<OrderListViewModel> logger,
                                  OrderService orderService,
                                  WindowManagerService windowService,
                                  NavigationService navigationService,
                                  OrderCollection orderCollection)
            : base()
        {
            _logger = logger;
            _orderService = orderService;
            _windowService = windowService;
            _navigationService = navigationService;
            _collection = orderCollection;
            Items = _collection;
        }

        public OrderListArgs ViewModelArgs { get; private set; }

        public async Task LoadAsync(OrderListArgs args, bool silent = false)
        {
            ViewModelArgs = args ?? new OrderListArgs();
            Query = ViewModelArgs.Query;

            if (silent)
            {
                await RefreshAsync();
            }
            else
            {
                StartStatusMessage("Loading orders...");
                if (await RefreshAsync())
                {
                    EndStatusMessage("Orders loaded");
                }
            }
        }

        public void Unload()
        {
            ViewModelArgs.Query = Query;
        }

        public void Subscribe()
        {
            Messenger.Register<ViewModelsMessage<OrderDto>>(this, OnMessage);
        }

        public void Unsubscribe()
        {
            Messenger.UnregisterAll(this);
        }

        public OrderListArgs CreateArgs()
        {
            return new OrderListArgs
            {
                Query = Query,
                OrderBy = ViewModelArgs.OrderBy,
                OrderByDesc = ViewModelArgs.OrderByDesc,
                CustomerId = ViewModelArgs.CustomerId
            };
        }

        public async Task<bool> RefreshAsync()
        {
            bool isOk = true;
            ItemsCount = 0;

            //todo: questa try é forse inutile, verificare virtualcollection loadasync
            try
            {
                DataRequest<Order> request = BuildDataRequest();
                await _collection.LoadAsync(request);
            }
            catch (Exception ex)
            {
                Items = new List<OrderDto>();
                StatusError($"Error loading Orders: {ex.Message}");
                _logger.LogError(LogEvents.Refresh, ex, "Error loading Orders");
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
                await _windowService.OpenWindow(typeof(OrderPage), new OrderDetailsArgs { OrderId = SelectedItem.Id });
            }
        }

        protected override async void OnNew()
        {
            if (IsMainView)
            {
                await _windowService.OpenWindow(typeof(OrderPage), new OrderDetailsArgs { CustomerId = ViewModelArgs.CustomerId });
            }
            else
            {
                _navigationService.Navigate(typeof(OrderPage), new OrderDetailsArgs { CustomerId = ViewModelArgs.CustomerId });
            }

            StatusReady();
        }

        protected override async void OnRefresh()
        {
            StartStatusMessage("Loading orders...");
            if (await RefreshAsync())
            {
                EndStatusMessage("Orders loaded");
            }
        }

        protected override async void OnDeleteSelection()
        {
            StatusReady();
            if (await _windowService.OpenDialog("Confirm Delete", "Are you sure you want to delete selected orders?", "Ok", "Cancel"))
            {
                int count = 0;
                try
                {
                    if (SelectedIndexRanges != null)
                    {
                        count = SelectedIndexRanges.Sum(r => r.Length);
                        StartStatusMessage($"Deleting {count} orders...");
                        await DeleteRangesAsync(SelectedIndexRanges);
                        Messenger.Send(new ViewModelsMessage<OrderDto>("ItemRangesDeleted", SelectedIndexRanges));
                    }
                    else if (SelectedItems != null)
                    {
                        count = SelectedItems.Count();
                        StartStatusMessage($"Deleting {count} orders...");
                        await DeleteItemsAsync(SelectedItems);
                        Messenger.Send(new ViewModelsMessage<OrderDto>("ItemsDeleted", SelectedItems));
                    }
                }
                catch (Exception ex)
                {
                    StatusError($"Error deleting {count} Orders: {ex.Message}");
                    _logger.LogError(LogEvents.Delete, ex, $"Error deleting {count} Orders");
                    count = 0;
                }
                await RefreshAsync();
                SelectedIndexRanges = null;
                SelectedItems = null;
                if (count > 0)
                {
                    EndStatusMessage($"{count} orders deleted");
                }
            }
        }

        private async Task DeleteItemsAsync(IEnumerable<OrderDto> models)
        {
            foreach (var model in models)
            {
                await _orderService.DeleteOrderAsync(model);
            }
        }

        private async Task DeleteRangesAsync(IEnumerable<IndexRange> ranges)
        {
            DataRequest<Order> request = BuildDataRequest();
            foreach (var range in ranges)
            {
                await _orderService.DeleteOrderRangeAsync(range.Index, range.Length, request);
            }
        }

        private DataRequest<Order> BuildDataRequest()
        {
            var request = new DataRequest<Order>()
            {
                Query = Query,
                OrderBy = ViewModelArgs.OrderBy,
                OrderByDesc = ViewModelArgs.OrderByDesc
            };
            if (ViewModelArgs.CustomerId > 0)
            {
                request.Where = (r) => r.CustomerId == ViewModelArgs.CustomerId;
            }
            return request;
        }

        private async void OnMessage(object recipient, ViewModelsMessage<OrderDto> message)
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
