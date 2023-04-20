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

using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using Inventory.Application;
using Inventory.Domain.Model;
using Inventory.Uwp.ViewModels.Common;
using Inventory.Uwp.ViewModels.Message;
using Microsoft.Extensions.Logging;

namespace Inventory.Uwp.ViewModels.Products
{
    public class ProductsViewModel : ViewModelBase
    {
        private readonly ILogger<ProductsViewModel> _logger;
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

        public ProductListViewModel ProductList
        {
            get; set;
        }

        public ProductDetailsViewModel ProductDetails
        {
            get; set;
        }

        public async Task LoadAsync(ProductListArgs args)
        {
            await ProductList.LoadAsync(args);
        }

        public void Unload()
        {
            ProductDetails.CancelEdit();
            ProductList.Unload();
        }

        public void Subscribe()
        {
            //MessageService.Subscribe<ProductListViewModel>(this, OnMessage);
            Messenger.Register<ProductChangeMessage>(this, OnMessage);
            ProductList.Subscribe();
            ProductDetails.Subscribe();
        }

        public void Unsubscribe()
        {
            //MessageService.Unsubscribe(this);
            Messenger.UnregisterAll(this);
            ProductList.Unsubscribe();
            ProductDetails.Unsubscribe();
        }

        private async void OnMessage(object recipient, ProductChangeMessage message)
        {
            if (message.Value == "ItemSelected")
            {
                if (message.Id != 0)
                {
                    //TODO: rendere il metodo OnItemSelected cancellabile
                    await OnItemSelected();
                }
            }
        }

        private async Task OnItemSelected()
        {
            if (ProductDetails.IsEditMode)
            {
                StatusReady();
                ProductDetails.CancelEdit();
            }
            var selected = ProductList.SelectedItem;
            if (!ProductList.IsMultipleSelection)
            {
                if (selected != null && !selected.IsEmpty)
                {
                    await PopulateDetails(selected);
                }
            }
            //ProductDetails.Item = selected;
        }

        private async Task PopulateDetails(Product selected)
        {
            try
            {
                //var model = await _productService.GetProductAsync(selected.Id);
                //selected.Merge(model);
                await ProductDetails.LoadAsync(new ProductDetailsArgs { ProductID = selected.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Load Details");
            }
        }
    }
}