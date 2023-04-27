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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Inventory.Domain.Model;
using Inventory.Domain.Repository;
using Inventory.Infrastructure.Common;
using Inventory.Infrastructure.Logging;
using Inventory.Uwp.Library.Common;
using Inventory.Uwp.Services;
using Inventory.Uwp.Services.VirtualCollections;
using Inventory.Uwp.ViewModels.Common;
using Inventory.Uwp.ViewModels.Message;
using Inventory.Uwp.Views.Orders;
using Microsoft.Extensions.Logging;

namespace Inventory.Uwp.ViewModels.Orders
{
    public class OrderListViewModel : GenericListViewModel<Order>
    {
        private readonly ILogger _logger;
        private readonly IOrderRepository _orderRepository;
        private readonly WindowManagerService _windowService;
        private readonly NavigationService _navigationService;
        private readonly OrderCollection _collection;

        public OrderListViewModel(ILogger<OrderListViewModel> logger,
                                  IOrderRepository orderRepository,
                                  WindowManagerService windowService,
                                  NavigationService navigationService,
                                  OrderCollection collection)
            : base()
        {
            _logger = logger;
            _orderRepository = orderRepository;
            _windowService = windowService;
            _navigationService = navigationService;
            _collection = collection;
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
            //MessageService.Subscribe<OrderListViewModel>(this, OnMessage);
            //MessageService.Subscribe<OrderDetailsViewModel>(this, OnMessage);
            Messenger.Register<OrderChangedMessage>(this, OnMessage);
        }

        public void Unsubscribe()
        {
            //MessageService.Unsubscribe(this);
            Messenger.UnregisterAll(this);
        }

        public OrderListArgs CreateArgs()
        {
            return new OrderListArgs
            {
                Query = Query,
                OrderBy = ViewModelArgs.OrderBy,
                OrderByDesc = ViewModelArgs.OrderByDesc,
                CustomerID = ViewModelArgs.CustomerID
            };
        }

        public async Task<bool> RefreshAsync()
        {
            var isOk = true;
            ItemsCount = 0;

            //todo: questa try é forse inutile, verificare virtualcollection loadasync
            try
            {
                DataRequest<Order> request = BuildDataRequest();
                await _collection.LoadAsync(request);
            }
            catch (Exception ex)
            {
                Items = new List<Order>();
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
                await _windowService.OpenInNewWindow<OrderPage>(new OrderDetailsArgs { OrderID = SelectedItem.Id });
            }
        }

        protected async override void OnNew()
        {
            if (IsMainView)
            {
                await _windowService.OpenInNewWindow<OrderPage>(new OrderDetailsArgs { CustomerID = ViewModelArgs.CustomerID });
            }
            else
            {
                _navigationService.Navigate<OrderPage>(new OrderDetailsArgs { CustomerID = ViewModelArgs.CustomerID });
            }

            StatusReady();
        }

        protected async override void OnRefresh()
        {
            StartStatusMessage("Loading orders...");
            if (await RefreshAsync())
            {
                EndStatusMessage("Orders loaded");
            }
        }

        protected async override void OnDeleteSelection()
        {
            StatusReady();
            if (await ShowDialogAsync("Confirm Delete", "Are you sure you want to delete selected orders?", "Ok", "Cancel"))
            {
                var count = 0;
                try
                {
                    if (SelectedIndexRanges != null)
                    {
                        count = SelectedIndexRanges.Sum(r => r.Length);
                        StartStatusMessage($"Deleting {count} orders...");
                        await DeleteRangesAsync(SelectedIndexRanges);
                        //MessageService.Send(this, "ItemRangesDeleted", SelectedIndexRanges);
                        Messenger.Send(new OrderChangedMessage("ItemRangesDeleted", SelectedIndexRanges));
                    }
                    else if (SelectedItems != null)
                    {
                        count = SelectedItems.Count();
                        StartStatusMessage($"Deleting {count} orders...");
                        await DeleteItemsAsync(SelectedItems);
                        //MessageService.Send(this, "ItemsDeleted", SelectedItems);
                        Messenger.Send(new OrderChangedMessage("ItemsDeleted", SelectedItems));
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

        private async Task DeleteItemsAsync(IEnumerable<Order> models)
        {
            foreach (var model in models)
            {
                await _orderRepository.DeleteOrdersAsync(model);
            }
        }

        private async Task DeleteRangesAsync(IEnumerable<IndexRange> ranges)
        {
            DataRequest<Order> request = BuildDataRequest();
            foreach (var range in ranges)
            {
                //await _orderService.DeleteOrderRangeAsync(range.Index, range.Length, request);
                var items = await _orderRepository.GetOrderKeysAsync(range.Index, range.Length, request);
                await _orderRepository.DeleteOrdersAsync(items.ToArray());
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
            if (ViewModelArgs.CustomerID > 0)
            {
                request.Where = (r) => r.CustomerId == ViewModelArgs.CustomerID;
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

        private async void OnMessage(object recipient, OrderChangedMessage message)
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

        protected override void SendItemChangedMessage(string message, long itemId)
            => Messenger.Send(new  OrderChangedMessage(message, itemId));
    }
}
