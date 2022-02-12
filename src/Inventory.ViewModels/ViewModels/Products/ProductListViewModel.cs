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
    #region ProductListArgs
    public class ProductListArgs
    {
        static public ProductListArgs CreateEmpty() => new ProductListArgs { IsEmpty = true };

        public ProductListArgs()
        {
            OrderBy = r => r.Name;
        }

        public bool IsEmpty { get; set; }

        public string Query { get; set; }

        public Expression<Func<Product, object>> OrderBy { get; set; }
        public Expression<Func<Product, object>> OrderByDesc { get; set; }
    }
    #endregion

    public class ProductListViewModel : ObservableRecipient //GenericListViewModel<ProductModel>
    {
        private readonly ILogger<ProductListViewModel> logger;
        private readonly IMessageService messageService;
        private readonly IContextService contextService;
        private readonly IDialogService dialogService;
        private readonly INavigationService navigationService;
        private readonly IProductService productService;

        public ProductListViewModel(ILogger<ProductListViewModel> logger,
                                    IMessageService messageService,
                                    IContextService contextService,
                                    IDialogService dialogService,
                                    INavigationService navigationService,
                                    IProductService productService) 
        {
            this.logger = logger;
            this.messageService = messageService;
            this.contextService = contextService;
            this.dialogService = dialogService;
            this.navigationService = navigationService;
            this.productService = productService;
        }

        public ICommand NewCommand => new RelayCommand(OnNew);

        public ICommand RefreshCommand => new RelayCommand(OnRefresh);

        public ICommand StartSelectionCommand => new RelayCommand(OnStartSelection);
        virtual protected void OnStartSelection()
        {
            //StatusMessage("Start selection");
            SelectedItem = null;
            SelectedItems = new List<ProductModel>();
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
                SelectedItems.AddRange(items.Cast<ProductModel>());
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
                foreach (ProductModel item in items)
                {
                    SelectedItems.Remove(item);
                }
                //StatusMessage($"{SelectedItems.Count} items selected");
            }
        }
        public ICommand SelectRangesCommand => new RelayCommand<IndexRange[]>(OnSelectRanges);
        virtual protected void OnSelectRanges(IndexRange[] indexRanges)
        {
            SelectedIndexRanges = indexRanges;
            int count = SelectedIndexRanges?.Sum(r => r.Length) ?? 0;
            //StatusMessage($"{count} items selected");
        }

        public ICommand DeleteSelectionCommand => new RelayCommand(OnDeleteSelection);







        public string Title => String.IsNullOrEmpty(Query) ? $" ({ItemsCount})" : $" ({ItemsCount} for \"{Query}\")";

        public bool IsMainView => contextService.IsMainView;

        public ProductListArgs ViewModelArgs { get; private set; }

        public ICommand ItemInvokedCommand => new RelayCommand<ProductModel>(ItemInvoked);
        private async void ItemInvoked(ProductModel model)
        {
            await navigationService.CreateNewViewAsync<ProductDetailsViewModel>(new ProductDetailsArgs { ProductID = model.ProductID });
        }

        public async Task LoadAsync(ProductListArgs args)
        {
            ViewModelArgs = args ?? ProductListArgs.CreateEmpty();
            Query = ViewModelArgs.Query;

            //StartStatusMessage("Loading products...");
            if (await RefreshAsync())
            {
                //EndStatusMessage("Products loaded");
            }
        }
        public void Unload()
        {
            ViewModelArgs.Query = Query;
        }

        public void Subscribe()
        {
            messageService.Subscribe<ProductListViewModel>(this, OnMessage);
            messageService.Subscribe<ProductDetailsViewModel>(this, OnMessage);
        }
        public void Unsubscribe()
        {
            messageService.Unsubscribe(this);
        }

        public ProductListArgs CreateArgs()
        {
            return new ProductListArgs
            {
                Query = Query,
                OrderBy = ViewModelArgs.OrderBy,
                OrderByDesc = ViewModelArgs.OrderByDesc
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
                Items = new List<ProductModel>();
                //StatusError($"Error loading Products: {ex.Message}");
                messageService.Send(this, "StatusError", $"Error loading Products: {ex.Message}");

                //LogException("Products", "Refresh", ex);
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

        private async Task<IList<ProductModel>> GetItemsAsync()
        {
            if (!ViewModelArgs.IsEmpty)
            {
                DataRequest<Product> request = BuildDataRequest();
                return await productService.GetProductsAsync(request);
            }
            return new List<ProductModel>();
        }

        protected async void OnNew()
        {

            if (IsMainView)
            {
                await navigationService.CreateNewViewAsync<ProductDetailsViewModel>(new ProductDetailsArgs());
            }
            else
            {
                navigationService.Navigate<ProductDetailsViewModel>(new ProductDetailsArgs());
            }

            //StatusReady();
            messageService.Send(this, "StatusMessage", "Ready");
        }

        protected  async void OnRefresh()
        {
            //StartStatusMessage("Loading products...");
            if (await RefreshAsync())
            {
                //EndStatusMessage("Products loaded");
            }
        }

        protected  async void OnDeleteSelection()
        {
            //StatusReady();
            messageService.Send(this, "StatusMessage", "Ready");
            if (await dialogService.ShowAsync("Confirm Delete", "Are you sure you want to delete selected products?", "Ok", "Cancel"))
            {
                int count = 0;
                //try
                //{
                    if (SelectedIndexRanges != null)
                    {
                        count = SelectedIndexRanges.Sum(r => r.Length);
                        //StartStatusMessage($"Deleting {count} products...");
                        await DeleteRangesAsync(SelectedIndexRanges);
                    messageService.Send(this, "ItemRangesDeleted", SelectedIndexRanges);
                    }
                    else if (SelectedItems != null)
                    {
                        count = SelectedItems.Count();
                        //StartStatusMessage($"Deleting {count} products...");
                        await DeleteItemsAsync(SelectedItems);
                    messageService.Send(this, "ItemsDeleted", SelectedItems);
                    }
                //}
                //catch (Exception ex)
                //{
                //    StatusError($"Error deleting {count} Products: {ex.Message}");
                //    LogException("Products", "Delete", ex);
                //    count = 0;
                //}
                await RefreshAsync();
                SelectedIndexRanges = null;
                SelectedItems = null;
                if (count > 0)
                {
                    //EndStatusMessage($"{count} products deleted");
                }
            }
        }

        private async Task DeleteItemsAsync(IEnumerable<ProductModel> models)
        {
            foreach (var model in models)
            {
                await productService.DeleteProductAsync(model);
            }
        }

        private async Task DeleteRangesAsync(IEnumerable<IndexRange> ranges)
        {
            DataRequest<Product> request = BuildDataRequest();
            foreach (var range in ranges)
            {
                await productService.DeleteProductRangeAsync(range.Index, range.Length, request);
            }
        }

        private DataRequest<Product> BuildDataRequest()
        {
            return new DataRequest<Product>()
            {
                Query = Query,
                OrderBy = ViewModelArgs.OrderBy,
                OrderByDesc = ViewModelArgs.OrderByDesc
            };
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

        private IList<ProductModel> _items = null;
        public IList<ProductModel> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        private ProductModel _selectedItem = default(ProductModel);
        public ProductModel SelectedItem
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

        private int _itemsCount = 0;
        public int ItemsCount
        {
            get => _itemsCount;
            set => SetProperty(ref _itemsCount, value);
        }

        public List<ProductModel> SelectedItems { get; protected set; }

        public IndexRange[] SelectedIndexRanges { get; protected set; }





    }
}
