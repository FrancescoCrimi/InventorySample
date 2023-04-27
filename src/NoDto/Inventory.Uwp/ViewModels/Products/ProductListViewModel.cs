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
using Inventory.Uwp.Views.Products;
using Microsoft.Extensions.Logging;

namespace Inventory.Uwp.ViewModels.Products
{
    public class ProductListViewModel : GenericListViewModel<Product>
    {
        private readonly ILogger _logger;
        private readonly IProductRepository _productRepository;
        private readonly NavigationService _navigationService;
        private readonly WindowManagerService _windowService;
        private readonly ProductCollection _collection;

        public ProductListViewModel(ILogger<ProductListViewModel> logger,
                                    IProductRepository productRepository,
                                    NavigationService navigationService,
                                    WindowManagerService windowService,
                                    ProductCollection collection)
            : base()
        {
            _logger = logger;
            _productRepository = productRepository;
            _navigationService = navigationService;
            _windowService = windowService;
            _collection = collection;
            Items = _collection;
        }

        public ProductListArgs ViewModelArgs
        {
            get; private set;
        }

        public ICommand ItemInvokedCommand => new RelayCommand<Product>(ItemInvoked);

        private async void ItemInvoked(Product model)
        {
            await _windowService.OpenInNewWindow<ProductPage>(new ProductDetailsArgs { ProductID = model.Id });
        }

        public async Task LoadAsync(ProductListArgs args)
        {
            ViewModelArgs = args ?? new ProductListArgs();
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
            //MessageService.Subscribe<ProductDetailsViewModel>(this, OnMessage);
            Messenger.Register<ProductChangeMessage>(this, OnMessage);
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
            var isOk = true;
            ItemsCount = 0;

            try
            {
                DataRequest<Product> request = BuildDataRequest();
                await _collection.LoadAsync(request);
            }
            catch (Exception ex)
            {
                Items = new List<Product>();
                StatusError($"Error loading Products: {ex.Message}");
                _logger.LogError(LogEvents.Refresh, ex, "Error loading Products");
                isOk = false;
            }

            ItemsCount = Items.Count;
            OnPropertyChanged(nameof(Title));
            return isOk;
        }

        protected async override void OnNew()
        {

            if (IsMainView)
            {
                await _windowService.OpenInNewWindow<ProductPage>(new ProductDetailsArgs());
            }
            else
            {
                _navigationService.Navigate<ProductPage>(new ProductDetailsArgs());
            }

            StatusReady();
        }

        protected async override void OnRefresh()
        {
            StartStatusMessage("Loading products...");
            if (await RefreshAsync())
            {
                EndStatusMessage("Products loaded");
            }
        }

        protected async override void OnDeleteSelection()
        {
            StatusReady();
            if (await ShowDialogAsync("Confirm Delete", "Are you sure you want to delete selected products?", "Ok", "Cancel"))
            {
                var count = 0;
                try
                {
                    if (SelectedIndexRanges != null)
                    {
                        count = SelectedIndexRanges.Sum(r => r.Length);
                        StartStatusMessage($"Deleting {count} products...");
                        await DeleteRangesAsync(SelectedIndexRanges);
                        //MessageService.Send(this, "ItemRangesDeleted", SelectedIndexRanges);
                        Messenger.Send(new ProductChangeMessage("ItemRangesDeleted", SelectedIndexRanges));
                    }
                    else if (SelectedItems != null)
                    {
                        count = SelectedItems.Count();
                        StartStatusMessage($"Deleting {count} products...");
                        await DeleteItemsAsync(SelectedItems);
                        //MessageService.Send(this, "ItemsDeleted", SelectedItems);
                        Messenger.Send(new ProductChangeMessage("ItemsDeleted", SelectedItems));
                    }
                }
                catch (Exception ex)
                {
                    StatusError($"Error deleting {count} Products: {ex.Message}");
                    _logger.LogError(LogEvents.Delete, ex, $"Error deleting {count} Products");
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

        private async Task DeleteItemsAsync(IEnumerable<Product> models)
        {
            foreach (var model in models)
            {
                await _productRepository.DeleteProductsAsync(model);
            }
        }

        private async Task DeleteRangesAsync(IEnumerable<IndexRange> ranges)
        {
            DataRequest<Product> request = BuildDataRequest();
            foreach (var range in ranges)
            {
                //await _productService.DeleteProductRangeAsync(range.Index, range.Length, request);
                var items = await _productRepository.GetProductKeysAsync(range.Index, range.Length, request);
                await _productRepository.DeleteProductsAsync(items.ToArray());
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

        private async void OnMessage(object recipient, ProductChangeMessage message)
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

        protected override void SendItemChangedMessage(string message, long itemId)
            => Messenger.Send(new ProductChangeMessage(message, itemId));
    }
}
