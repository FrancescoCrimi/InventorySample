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
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Inventory.Uwp.Library.Common;
using Inventory.Infrastructure.Common;
using Inventory.Domain.Model;
using Inventory.Uwp.ViewModels;
using Inventory.Uwp.Dto;
using Inventory.Uwp.Services;
using Inventory.Uwp.ViewModels.Common;
using Inventory.Uwp.Services.VirtualCollections;
using Inventory.Uwp.Views.Orders;

namespace Inventory.Uwp.ViewModels.Orders
{
    public class OrderListViewModel : GenericListViewModel<OrderDto>
    {
        private readonly ILogger<OrderListViewModel> _logger;
        private readonly OrderServiceFacade _orderService;
        private readonly WindowManagerService _windowService;
        private readonly NavigationService _navigationService;
        private readonly OrderCollection _collection;

        public OrderListViewModel(ILogger<OrderListViewModel> logger,
                                  OrderServiceFacade orderService,
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
            //MessageService.Subscribe<OrderListViewModel>(this, OnMessage);
            Messenger.Register<ItemMessage<IList<OrderDto>>>(this, OnOrderListMessage);
            Messenger.Register<ItemMessage<IList<IndexRange>>>(this, OnIndexRangeListMessage);

            //MessageService.Subscribe<OrderDetailsViewModel>(this, OnMessage);
            Messenger.Register<ItemMessage<OrderDto>>(this, OnOrderMessage);
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
            bool isOk = true;
            ItemsCount = 0;

            try
            {
                DataRequest<Order> request = BuildDataRequest();
                await _collection.LoadAsync(request);
            }
            catch (Exception ex)
            {
                Items = new List<OrderDto>();
                StatusError($"Error loading Orders: {ex.Message}");
                _logger.LogError(ex, "Refresh");
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

        protected override async void OnNew()
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
            if (await ShowDialogAsync("Confirm Delete", "Are you sure you want to delete selected orders?", "Ok", "Cancel"))
            {
                int count = 0;
                try
                {
                    if (SelectedIndexRanges != null)
                    {
                        count = SelectedIndexRanges.Sum(r => r.Length);
                        StartStatusMessage($"Deleting {count} orders...");
                        await DeleteRangesAsync(SelectedIndexRanges);
                        //MessageService.Send(this, "ItemRangesDeleted", SelectedIndexRanges);
                        Messenger.Send(new ItemMessage<IList<IndexRange>>(SelectedIndexRanges, "ItemRangesDeleted"));
                    }
                    else if (SelectedItems != null)
                    {
                        count = SelectedItems.Count();
                        StartStatusMessage($"Deleting {count} orders...");
                        await DeleteItemsAsync(SelectedItems);
                        //MessageService.Send(this, "ItemsDeleted", SelectedItems);
                        Messenger.Send(new ItemMessage<IList<OrderDto>>(SelectedItems, "ItemsDeleted"));
                    }
                }
                catch (Exception ex)
                {
                    StatusError($"Error deleting {count} Orders: {ex.Message}");
                    _logger.LogError(ex, "Delete");
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
        private async void OnOrderMessage(object recipient, ItemMessage<OrderDto> message)
        {
            switch (message.Message)
            {
                case "NewItemSaved":
                case "ItemDeleted":
                    //case "ItemsDeleted":
                    //case "ItemRangesDeleted":
                    //await ContextService.RunAsync(async () =>
                    //{
                    await RefreshAsync();
                    //});
                    break;
            }
        }

        private async void OnIndexRangeListMessage(object recipient, ItemMessage<IList<IndexRange>> message)
        {
            switch (message.Message)
            {
                //case "NewItemSaved":
                //case "ItemDeleted":
                //case "ItemsDeleted":
                case "ItemRangesDeleted":
                    //await ContextService.RunAsync(async () =>
                    //{
                    await RefreshAsync();
                    //});
                    break;
            }
        }

        private async void OnOrderListMessage(object recipient, ItemMessage<IList<OrderDto>> message)
        {
            switch (message.Message)
            {
                //case "NewItemSaved":
                //case "ItemDeleted":
                case "ItemsDeleted":
                    //case "ItemRangesDeleted":
                    //await ContextService.RunAsync(async () =>
                    //{
                    await RefreshAsync();
                    //});
                    break;
            }
        }
    }
}
