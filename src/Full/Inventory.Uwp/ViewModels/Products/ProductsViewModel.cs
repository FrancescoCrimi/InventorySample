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

using CommunityToolkit.Mvvm.Messaging;
using Inventory.Infrastructure.Logging;
using Inventory.Uwp.Dto;
using Inventory.Uwp.Services;
using Inventory.Uwp.ViewModels.Common;
using Inventory.Uwp.ViewModels.Message;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Inventory.Uwp.ViewModels.Products
{
    public class ProductsViewModel : ViewModelBase
    {
        private readonly ILogger _logger;
        private readonly ProductService _productService;

        public ProductsViewModel(ILogger<ProductsViewModel> logger,
                                 ProductService productService,
                                 ProductListViewModel productListViewModel,
                                 ProductDetailsViewModel productDetailsViewModel)
            : base()
        {
            _logger = logger;
            _productService = productService;

            ProductList = productListViewModel;
            ProductDetails = productDetailsViewModel;
        }

        public ProductListViewModel ProductList { get; set; }
        public ProductDetailsViewModel ProductDetails { get; set; }

        public async Task LoadAsync(ProductListArgs args)
        {
            await ProductList.LoadAsync(args);
            if (args != null)
            {
                IsMainView = args.IsMainView;
                ProductList.IsMainView = args.IsMainView;
                ProductDetails.IsMainView = args.IsMainView;
            }
        }
        public void Unload()
        {
            ProductDetails.CancelEdit();
            ProductList.Unload();
        }

        public void Subscribe()
        {
            Messenger.Register<ViewModelsMessage<ProductDto>>(this, OnMessage);
            ProductList.Subscribe();
            ProductDetails.Subscribe();
        }

        public void Unsubscribe()
        {
            Messenger.UnregisterAll(this);
            ProductList.Unsubscribe();
            ProductDetails.Unsubscribe();
        }

        private async void OnMessage(object recipient, ViewModelsMessage<ProductDto> message)
        {
            if (message.Value == "ItemSelected")
            {
                if (message.Id != 0)
                {
                    //TODO: rendere il metodo OnItemSelected cancellabile
                    await OnItemSelected(message.Id);
                }
            }
        }

        private async Task OnItemSelected(long id)
        {
            if (ProductDetails.IsEditMode)
            {
                StatusReady();
                ProductDetails.CancelEdit();
            }
            if (!ProductList.IsMultipleSelection)
            {
                if (id != 0)
                {
                    await PopulateDetails(id);
                }
            }
        }

        private async Task PopulateDetails(long id)
        {
            try
            {
                await ProductDetails.LoadAsync(new ProductDetailsArgs { ProductId = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(LogEvents.LoadDetails, ex, "Load Product Details");
            }
        }
    }
}
