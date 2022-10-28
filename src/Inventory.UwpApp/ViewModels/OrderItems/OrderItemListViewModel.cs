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

using CiccioSoft.Inventory.Domain.Model;
using CiccioSoft.Inventory.Infrastructure.Common;
using Inventory.UwpApp.Models;
using Inventory.UwpApp.Services;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Inventory.UwpApp.Views.OrderItem;
using Inventory.UwpApp.Library.Common;

namespace Inventory.UwpApp.ViewModels
{
    #region OrderItemListArgs
    public class OrderItemListArgs
    {
        static public OrderItemListArgs CreateEmpty() => new OrderItemListArgs { IsEmpty = true };

        public OrderItemListArgs()
        {
            OrderBy = r => r.OrderLine;
        }

        public long OrderID { get; set; }

        public bool IsEmpty { get; set; }

        public string Query { get; set; }

        public Expression<Func<OrderItem, object>> OrderBy { get; set; }
        public Expression<Func<OrderItem, object>> OrderByDesc { get; set; }
    }
    #endregion

    public class OrderItemListViewModel : GenericListViewModel<OrderItemModel>
    {
        private readonly ILogger<OrderItemListViewModel> logger;
        private readonly OrderItemServiceUwp orderItemService;
        private readonly NavigationService navigationService;
        private readonly WindowService windowService;

        public OrderItemListViewModel(ILogger<OrderItemListViewModel> logger,
                                      OrderItemServiceUwp orderItemService,
                                      NavigationService navigationService,
                                      WindowService windowService)
            : base()
        {
            this.logger = logger;
            this.orderItemService = orderItemService;
            this.navigationService = navigationService;
            this.windowService = windowService;
        }

        public OrderItemListArgs ViewModelArgs { get; private set; }

        public async Task LoadAsync(OrderItemListArgs args, bool silent = false)
        {
            ViewModelArgs = args ?? OrderItemListArgs.CreateEmpty();
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
            Messenger.Register<ItemMessage<IList<OrderItemModel>>>(this, OnOrderItemListMessage);
            Messenger.Register<ItemMessage<IList<IndexRange>>>(this, OnIndexRangeListMessage);

            //MessageService.Subscribe<OrderItemDetailsViewModel>(this, OnMessage);
            Messenger.Register<ItemMessage<OrderItemModel>>(this, OnOrderItemMessage);
        }

        public void Unsubscribe()
        {
            //MessageService.Unsubscribe(this);
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

            try
            {
                Items = await GetItemsAsync();
            }
            catch (Exception ex)
            {
                Items = new List<OrderItemModel>();
                StatusError($"Error loading Order items: {ex.Message}");
                logger.LogError(ex, "Refresh");
                isOk = false;
            }

            ItemsCount = Items.Count;
            if (!IsMultipleSelection)
            {
                SelectedItem = Items.FirstOrDefault();
            }
            OnPropertyChanged(nameof(Title));

            return isOk;
        }

        private async Task<IList<OrderItemModel>> GetItemsAsync()
        {
            if (!ViewModelArgs.IsEmpty)
            {
                DataRequest<OrderItem> request = BuildDataRequest();
                return await orderItemService.GetOrderItemsAsync(request);
            }
            return new List<OrderItemModel>();
        }

        public ICommand OpenInNewViewCommand => new RelayCommand(OnOpenInNewView);
        private async void OnOpenInNewView()
        {
            if (SelectedItem != null)
            {
                await windowService.OpenInNewWindow<OrderItemDetailsViewModel>(new OrderItemDetailsArgs { OrderID = SelectedItem.OrderID, OrderLine = SelectedItem.OrderLine });
            }
        }

        protected override async void OnNew()
        {
            if (IsMainView)
            {
                await windowService.OpenInNewWindow<OrderItemDetailsViewModel>(new OrderItemDetailsArgs { OrderID = ViewModelArgs.OrderID });
            }
            else
            {
                navigationService.Navigate<OrderItemDetailsPage>(new OrderItemDetailsArgs { OrderID = ViewModelArgs.OrderID });
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
                        //MessageService.Send(this, "ItemRangesDeleted", SelectedIndexRanges);
                        Messenger.Send(new ItemMessage<IList<IndexRange>>(SelectedIndexRanges, "ItemRangesDeleted"));
                    }
                    else if (SelectedItems != null)
                    {
                        count = SelectedItems.Count();
                        StartStatusMessage($"Deleting {count} order items...");
                        await DeleteItemsAsync(SelectedItems);
                        //MessageService.Send(this, "ItemsDeleted", SelectedItems);
                        Messenger.Send(new ItemMessage<IList<OrderItemModel>>(SelectedItems, "ItemsDeleted"));
                    }
                }
                catch (Exception ex)
                {
                    StatusError($"Error deleting {count} order items: {ex.Message}");
                    logger.LogError(ex, "Delete");
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

        private async Task DeleteItemsAsync(IEnumerable<OrderItemModel> models)
        {
            foreach (var model in models)
            {
                await orderItemService.DeleteOrderItemAsync(model);
            }
        }

        private async Task DeleteRangesAsync(IEnumerable<IndexRange> ranges)
        {
            DataRequest<OrderItem> request = BuildDataRequest();
            foreach (var range in ranges)
            {
                await orderItemService.DeleteOrderItemRangeAsync(range.Index, range.Length, request);
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
                request.Where = (r) => r.OrderID == ViewModelArgs.OrderID;
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
        private async void OnOrderItemListMessage(object recipient, ItemMessage<IList<OrderItemModel>> message)
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
        private async void OnOrderItemMessage(object recipient, ItemMessage<OrderItemModel> message)
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
    }
}
