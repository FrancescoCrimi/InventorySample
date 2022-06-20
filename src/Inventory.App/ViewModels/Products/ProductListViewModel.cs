﻿#region copyright
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

using CiccioSoft.Inventory.Data;
using CiccioSoft.Inventory.Uwp.Models;
using CiccioSoft.Inventory.Uwp.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CiccioSoft.Inventory.Uwp.ViewModels
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

    public class ProductListViewModel : GenericListViewModel<ProductModel>
    {
        private readonly ILogger<ProductListViewModel> logger;
        private readonly IProductService productService;
        private readonly INavigationService navigationService;
        private readonly IWindowService windowService;
        private readonly IDialogService dialogService;

        public ProductListViewModel(ILogger<ProductListViewModel> logger,
                                    IProductService productService,
                                    INavigationService navigationService,
                                    IWindowService windowService,
                                    IDialogService dialogService)
            : base()
        {
            this.logger = logger;
            this.productService = productService;
            this.navigationService = navigationService;
            this.windowService = windowService;
            this.dialogService = dialogService;
        }

        public ProductListArgs ViewModelArgs { get; private set; }

        public ICommand ItemInvokedCommand => new RelayCommand<ProductModel>(ItemInvoked);
        private async void ItemInvoked(ProductModel model)
        {
            await windowService.OpenInNewWindow<ProductDetailsViewModel>(new ProductDetailsArgs { ProductID = model.ProductID });
        }

        public async Task LoadAsync(ProductListArgs args)
        {
            ViewModelArgs = args ?? ProductListArgs.CreateEmpty();
            Query = ViewModelArgs.Query;

            StartStatusMessage("Loading products...");
            if (await RefreshAsync())
            {
                EndStatusMessage("Products loaded");
            }
        }
        public void Unload()
        {
            ViewModelArgs.Query = Query;
        }

        public void Subscribe()
        {
            //MessageService.Subscribe<ProductListViewModel>(this, OnMessage);
            Messenger.Register<ItemMessage<IList<ProductModel>>>(this, OnProductListMessage);
            Messenger.Register<ItemMessage<IList<IndexRange>>>(this, OnIndexRangeListMessage);

            //MessageService.Subscribe<ProductDetailsViewModel>(this, OnMessage);
            Messenger.Register<ItemMessage<ProductModel>>(this, OnProductMessage);
        }

        public void Unsubscribe()
        {
            //MessageService.Unsubscribe(this);
            Messenger.UnregisterAll(this);
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
                StatusError($"Error loading Products: {ex.Message}");
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

        private async Task<IList<ProductModel>> GetItemsAsync()
        {
            if (!ViewModelArgs.IsEmpty)
            {
                DataRequest<Product> request = BuildDataRequest();
                var collection = new ProductCollection(productService);
                await collection.LoadAsync(request);
                return collection;
            }
            return new List<ProductModel>();
        }

        protected override async void OnNew()
        {

            if (IsMainView)
            {
                await windowService.OpenInNewWindow<ProductDetailsViewModel>(new ProductDetailsArgs());
            }
            else
            {
                navigationService.Navigate<ProductDetailsViewModel>(new ProductDetailsArgs());
            }

            StatusReady();
        }

        protected override async void OnRefresh()
        {
            StartStatusMessage("Loading products...");
            if (await RefreshAsync())
            {
                EndStatusMessage("Products loaded");
            }
        }

        protected override async void OnDeleteSelection()
        {
            StatusReady();
            if (await dialogService.ShowAsync("Confirm Delete", "Are you sure you want to delete selected products?", "Ok", "Cancel"))
            {
                int count = 0;
                try
                {
                    if (SelectedIndexRanges != null)
                    {
                        count = SelectedIndexRanges.Sum(r => r.Length);
                        StartStatusMessage($"Deleting {count} products...");
                        await DeleteRangesAsync(SelectedIndexRanges);
                        //MessageService.Send(this, "ItemRangesDeleted", SelectedIndexRanges);
                        Messenger.Send(new ItemMessage<IList<IndexRange>>(SelectedIndexRanges, "ItemRangesDeleted"));
                    }
                    else if (SelectedItems != null)
                    {
                        count = SelectedItems.Count();
                        StartStatusMessage($"Deleting {count} products...");
                        await DeleteItemsAsync(SelectedItems);
                        //MessageService.Send(this, "ItemsDeleted", SelectedItems);
                        Messenger.Send(new ItemMessage<IList<ProductModel>>(SelectedItems, "ItemsDeleted"));
                    }
                }
                catch (Exception ex)
                {
                    StatusError($"Error deleting {count} Products: {ex.Message}");
                    logger.LogError(ex, "Delete");
                    count = 0;
                }
                await RefreshAsync();
                SelectedIndexRanges = null;
                SelectedItems = null;
                if (count > 0)
                {
                    EndStatusMessage($"{count} products deleted");
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
        private async void OnProductListMessage(object recipient, ItemMessage<IList<ProductModel>> message)
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
        private async void OnProductMessage(object recipient, ItemMessage<ProductModel> message)
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