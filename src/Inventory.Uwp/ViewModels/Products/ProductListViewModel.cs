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
using Inventory.Interface.Dto;
using Inventory.Interface;
using Inventory.Uwp.Library.Common;
using Inventory.Uwp.Services;
using Inventory.Uwp.Services.VirtualCollections;
using Inventory.Uwp.ViewModels.Common;
using Inventory.Uwp.ViewModels.Message;
using Inventory.Uwp.Views.Products;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Inventory.Uwp.ViewModels.Products
{
    public class ProductListViewModel : GenericListViewModel<ProductDto>
    {
        private readonly ILogger _logger;
        private readonly IProductServiceFacade _productService;
        private readonly NavigationService _navigationService;
        private readonly WindowManagerService _windowService;
        private readonly ProductCollection _collection;

        public ProductListViewModel(ILogger<ProductListViewModel> logger,
                                    IProductServiceFacade productService,
                                    NavigationService navigationService,
                                    WindowManagerService windowService,
                                    ProductCollection collection)
            : base()
        {
            _logger = logger;
            _productService = productService;
            _navigationService = navigationService;
            _windowService = windowService;
            _collection = collection;
            Items = _collection;
        }

        public ProductListArgs ViewModelArgs { get; private set; }

        public ICommand ItemInvokedCommand => new RelayCommand<ProductDto>(ItemInvoked);

        private async void ItemInvoked(ProductDto model)
        {
            await _windowService.OpenWindow(typeof(ProductView), new ProductDetailsArgs { ProductId = model.Id });
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
            Messenger.Register<ViewModelsMessage<ProductDto>>(this, OnMessage);
        }

        public void Unsubscribe()
        {
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
            ItemsCount = 0;

            try
            {
                DataRequest<Product> request = BuildDataRequest();
                await _collection.LoadAsync(request);
            }
            catch (Exception ex)
            {
                Items = new List<ProductDto>();
                StatusError($"Error loading Products: {ex.Message}");
                _logger.LogError(LogEvents.Refresh, ex, "Error loading Products");
                isOk = false;
            }

            ItemsCount = Items.Count;
            OnPropertyChanged(nameof(Title));
            return isOk;
        }

        protected override async void OnNew()
        {

            if (IsMainView)
            {
                await _windowService.OpenWindow(typeof(ProductView), new ProductDetailsArgs());
            }
            else
            {
                _navigationService.Navigate(typeof(ProductView), new ProductDetailsArgs());
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
            if (await _windowService.OpenDialog("Confirm Delete", "Are you sure you want to delete selected products?", "Ok", "Cancel"))
            {
                int count = 0;
                try
                {
                    if (SelectedIndexRanges != null)
                    {
                        count = SelectedIndexRanges.Sum(r => r.Length);
                        StartStatusMessage($"Deleting {count} products...");
                        await DeleteRangesAsync(SelectedIndexRanges);
                        Messenger.Send(new ViewModelsMessage<ProductDto>("ItemRangesDeleted", SelectedIndexRanges));
                    }
                    else if (SelectedItems != null)
                    {
                        count = SelectedItems.Count();
                        StartStatusMessage($"Deleting {count} products...");
                        await DeleteItemsAsync(SelectedItems);
                        Messenger.Send(new ViewModelsMessage<ProductDto>("ItemsDeleted", SelectedItems));
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

        private async Task DeleteItemsAsync(IEnumerable<ProductDto> models)
        {
            foreach (var model in models)
            {
                await _productService.DeleteProductAsync(model);
            }
        }

        private async Task DeleteRangesAsync(IEnumerable<IndexRange> ranges)
        {
            DataRequest<Product> request = BuildDataRequest();
            foreach (var range in ranges)
            {
                await _productService.DeleteProductRangeAsync(range.Index, range.Length, request);
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

        private async void OnMessage(object recipient, ViewModelsMessage<ProductDto> message)
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
