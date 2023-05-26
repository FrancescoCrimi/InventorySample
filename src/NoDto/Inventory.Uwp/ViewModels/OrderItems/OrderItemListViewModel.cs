// Copyright (c) Microsoft. All rights reserved.
// Copyright (c) 2023 Francesco Crimi francrim@gmail.com
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Inventory.Domain.Aggregates.OrderAggregate;
using Inventory.Domain.Repository;
using Inventory.Infrastructure.Common;
using Inventory.Infrastructure.Logging;
using Inventory.Uwp.Library.Common;
using Inventory.Uwp.Services;
using Inventory.Uwp.ViewModels.Common;
using Inventory.Uwp.ViewModels.Message;
using Inventory.Uwp.Views.OrderItems;
using Microsoft.Extensions.Logging;

namespace Inventory.Uwp.ViewModels.OrderItems
{
    public class OrderItemListViewModel : GenericListViewModel<OrderItem>
    {
        private readonly ILogger _logger;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly NavigationService _navigationService;
        private readonly WindowManagerService _windowService;

        public OrderItemListViewModel(ILogger<OrderItemListViewModel> logger,
                                      IOrderItemRepository orderItemRepository,
                                      NavigationService navigationService,
                                      WindowManagerService windowService)
            : base()
        {
            _logger = logger;
            _orderItemRepository = orderItemRepository;
            _navigationService = navigationService;
            _windowService = windowService;
        }

        public OrderItemListArgs ViewModelArgs
        {
            get; private set;
        }

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
            //MessageService.Subscribe<OrderItemListViewModel>(this, OnMessage);
            //MessageService.Subscribe<OrderItemDetailsViewModel>(this, OnMessage);
            Messenger.Register<ViewModelsMessage<OrderItem>>(this, OnMessage);
        }

        public void Unsubscribe()
        {
            //MessageService.Unsubscribe(this);
            Messenger.UnregisterAll(this);
        }

        public OrderItemListArgs CreateArgs()
        {
            return new OrderItemListArgs
            {
                Query = Query,
                OrderBy = ViewModelArgs.OrderBy,
                OrderByDesc = ViewModelArgs.OrderByDesc,
                OrderId = ViewModelArgs.OrderId
            };
        }

        public async Task<bool> RefreshAsync()
        {
            var isOk = true;
            Items = null;
            ItemsCount = 0;
            SelectedItem = null;

            try
            {
                DataRequest<OrderItem> request = BuildDataRequest();
                Items = await _orderItemRepository.GetOrderItemsAsync(0, 100, request);
            }
            catch (Exception ex)
            {
                Items = new List<OrderItem>();
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

        protected async override void OnNew()
        {
            if (IsMainView)
            {
                await _windowService.OpenInNewWindow<OrderItemPage>(new OrderItemDetailsArgs { OrderID = ViewModelArgs.OrderId });
            }
            else
            {
                _navigationService.Navigate<OrderItemPage>(new OrderItemDetailsArgs { OrderID = ViewModelArgs.OrderId });
            }

            StatusReady();
        }

        protected async override void OnRefresh()
        {
            StartStatusMessage("Loading order items...");
            if (await RefreshAsync())
            {
                EndStatusMessage("Order items loaded");
            }
        }

        protected async override void OnDeleteSelection()
        {
            StatusReady();
            if (await ShowDialogAsync("Confirm Delete", "Are you sure you want to delete selected order items?", "Ok", "Cancel"))
            {
                var count = 0;
                try
                {
                    if (SelectedIndexRanges != null)
                    {
                        count = SelectedIndexRanges.Sum(r => r.Length);
                        StartStatusMessage($"Deleting {count} order items...");
                        await DeleteRangesAsync(SelectedIndexRanges);
                        //MessageService.Send(this, "ItemRangesDeleted", SelectedIndexRanges);
                        Messenger.Send(new ViewModelsMessage<OrderItem>("ItemRangesDeleted", SelectedIndexRanges));
                    }
                    else if (SelectedItems != null)
                    {
                        count = SelectedItems.Count();
                        StartStatusMessage($"Deleting {count} order items...");
                        await DeleteItemsAsync(SelectedItems);
                        //MessageService.Send(this, "ItemsDeleted", SelectedItems);
                        Messenger.Send(new ViewModelsMessage<OrderItem>("ItemsDeleted", SelectedItems));
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

        private async Task DeleteItemsAsync(IEnumerable<OrderItem> models)
        {
            foreach (var model in models)
            {
                await _orderItemRepository.DeleteOrderItemsAsync(model);
            }
        }

        private async Task DeleteRangesAsync(IEnumerable<IndexRange> ranges)
        {
            DataRequest<OrderItem> request = BuildDataRequest();
            foreach (var range in ranges)
            {
                //await _orderItemService.DeleteOrderItemRangeAsync(range.Index, range.Length, request);
                var items = await _orderItemRepository.GetOrderItemKeysAsync(range.Index, range.Length, request);
                await _orderItemRepository.DeleteOrderItemsAsync(items.ToArray());
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
            if (ViewModelArgs.OrderId > 0)
            {
                request.Where = (r) => r.OrderId == ViewModelArgs.OrderId;
            }
            return request;
        }

        //private async void OnMessage(ViewModelBase sender, string message, object args)
        //{
        //    switch (message)
        //    {
        //        case "NewItemSaved":
        //        case "ItemDeleted":
        //        case "ItemsDeleted":
        //        case "ItemRangesDeleted":
        //            //await ContextService.RunAsync(async () =>
        //            //{
        //                await RefreshAsync();
        //            //});
        //            break;
        //    }
        //}

        private async void OnMessage(object recipient, ViewModelsMessage<OrderItem> message)
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
