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
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

using Inventory.Data;
using Inventory.Models;
using Inventory.Services;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;

namespace Inventory.ViewModels
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

    public class OrderItemListViewModel : ObservableRecipient //GenericListViewModel<OrderItemModel>
    {
        private readonly ILogger<OrderItemListViewModel> logger;
        private readonly IMessageService messageService;
        private readonly IContextService contextService;
        private readonly IDialogService dialogService;
        private readonly INavigationService navigationService;
        private readonly IOrderItemService orderItemService;

        public OrderItemListViewModel(ILogger<OrderItemListViewModel> logger,
                                      IMessageService messageService,
                                      IContextService contextService,
                                      IDialogService dialogService,
                                      INavigationService navigationService,
                                      IOrderItemService orderItemService)
        {
            this.logger = logger;
            this.messageService = messageService;
            this.contextService = contextService;
            this.dialogService = dialogService;
            this.navigationService = navigationService;
            this.orderItemService = orderItemService;
        }




        public ICommand NewCommand => new RelayCommand(OnNew);

        public ICommand RefreshCommand => new RelayCommand(OnRefresh);

        public ICommand StartSelectionCommand => new RelayCommand(OnStartSelection);
        virtual protected void OnStartSelection()
        {
            //StatusMessage("Start selection");
            SelectedItem = null;
            SelectedItems = new List<OrderItemModel>();
            SelectedIndexRanges = null;
            IsMultipleSelection = true;
        }

        public ICommand CancelSelectionCommand => new RelayCommand(OnCancelSelection);
        virtual protected void OnCancelSelection()
        {
            //StatusReady();
            messageService.Send(this, "StatusMessage", "Ready");

            SelectedItems = null;
            SelectedIndexRanges = null;
            IsMultipleSelection = false;
            SelectedItem = Items?.FirstOrDefault();
        }

        public ICommand SelectItemsCommand => new RelayCommand<IList<object>>(OnSelectItems);
        virtual protected void OnSelectItems(IList<object> items)
        {
            messageService.Send(this, "StatusMessage", "Ready");
            if (IsMultipleSelection)
            {
                SelectedItems.AddRange(items.Cast<OrderItemModel>());
                //StatusMessage($"{SelectedItems.Count} items selected");
            }
        }

        public ICommand DeselectItemsCommand => new RelayCommand<IList<object>>(OnDeselectItems);
        virtual protected void OnDeselectItems(IList<object> items)
        {
            if (items?.Count > 0)
            {
                messageService.Send(this, "StatusMessage", "Ready");
            }
            if (IsMultipleSelection)
            {
                foreach (OrderItemModel item in items)
                {
                    SelectedItems.Remove(item);
                }
                //StatusMessage($"{SelectedItems.Count} items selected");
            }
        }

        public ICommand DeleteSelectionCommand => new RelayCommand(OnDeleteSelection);







        public string Title => String.IsNullOrEmpty(Query) ? $" ({ItemsCount})" : $" ({ItemsCount} for \"{Query}\")";

        public bool IsMainView => contextService.IsMainView;


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
                //StartStatusMessage("Loading order items...");
                if (await RefreshAsync())
                {
                    //EndStatusMessage("OrderItems loaded");
                }
            }
        }
        public void Unload()
        {
            ViewModelArgs.Query = Query;
        }

        public void Subscribe()
        {
            messageService.Subscribe<OrderItemListViewModel>(this, OnMessage);
            messageService.Subscribe<OrderItemDetailsViewModel>(this, OnMessage);
        }
        public void Unsubscribe()
        {
            messageService.Unsubscribe(this);
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
                //StatusError($"Error loading Order items: {ex.Message}");
                messageService.Send(this, "StatusError", $"Error loading Order items: {ex.Message}");

                //LogException("OrderItems", "Refresh", ex);
                logger.LogCritical(ex, "Refresh");

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
                await navigationService.CreateNewViewAsync<OrderItemDetailsViewModel>(new OrderItemDetailsArgs { OrderID = SelectedItem.OrderID, OrderLine = SelectedItem.OrderLine });
            }
        }

        protected  async void OnNew()
        {
            if (IsMainView)
            {
                await navigationService.CreateNewViewAsync<OrderItemDetailsViewModel>(new OrderItemDetailsArgs { OrderID = ViewModelArgs.OrderID });
            }
            else
            {
                navigationService.Navigate<OrderItemDetailsViewModel>(new OrderItemDetailsArgs { OrderID = ViewModelArgs.OrderID });
            }

            //StatusReady();
            messageService.Send(this, "StatusMessage", "Ready");
        }

        protected  async void OnRefresh()
        {
            //StartStatusMessage("Loading order items...");
            if (await RefreshAsync())
            {
                //EndStatusMessage("Order items loaded");
            }
        }

        protected  async void OnDeleteSelection()
        {
            messageService.Send(this, "StatusMessage", "Ready");
            //StatusReady();
            if (await dialogService.ShowAsync("Confirm Delete", "Are you sure you want to delete selected order items?", "Ok", "Cancel"))
            {
                int count = 0;
                //try
                //{
                    if (SelectedIndexRanges != null)
                    {
                        count = SelectedIndexRanges.Sum(r => r.Length);
                        //StartStatusMessage($"Deleting {count} order items...");
                        await DeleteRangesAsync(SelectedIndexRanges);
                    messageService.Send(this, "ItemRangesDeleted", SelectedIndexRanges);
                    }
                    else if (SelectedItems != null)
                    {
                        count = SelectedItems.Count();
                        //StartStatusMessage($"Deleting {count} order items...");
                        await DeleteItemsAsync(SelectedItems);
                    messageService.Send(this, "ItemsDeleted", SelectedItems);
                    }
                //}
                //catch (Exception ex)
                //{
                //    StatusError($"Error deleting {count} order items: {ex.Message}");
                //    LogException("OrderItems", "Delete", ex);
                //    count = 0;
                //}
                await RefreshAsync();
                SelectedIndexRanges = null;
                SelectedItems = null;
                if (count > 0)
                {
                    //EndStatusMessage($"{count} order items deleted");
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

        private async void OnMessage(ObservableRecipient sender, string message, object args)
        {
            switch (message)
            {
                case "NewItemSaved":
                case "ItemDeleted":
                case "ItemsDeleted":
                case "ItemRangesDeleted":
                    await contextService.RunAsync(async () =>
                    {
                        await RefreshAsync();
                    });
                    break;
            }
        }







        private string _query = null;
        public string Query
        {
            get => _query;
            set => SetProperty(ref _query, value);
        }

        private IList<OrderItemModel> _items = null;
        public IList<OrderItemModel> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        private int _itemsCount = 0;
        public int ItemsCount
        {
            get => _itemsCount;
            set => SetProperty(ref _itemsCount, value);
        }

        private OrderItemModel _selectedItem = default(OrderItemModel);
        public OrderItemModel SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (SetProperty(ref _selectedItem, value))
                {
                    if (!IsMultipleSelection)
                    {
                        messageService.Send(this, "ItemSelected", _selectedItem);
                    }
                }
            }
        }

        private bool _isMultipleSelection = false;
        public bool IsMultipleSelection
        {
            get => _isMultipleSelection;
            set => SetProperty(ref _isMultipleSelection, value);
        }

        public IndexRange[] SelectedIndexRanges { get; protected set; }

        public List<OrderItemModel> SelectedItems { get; protected set; }
    }
}
