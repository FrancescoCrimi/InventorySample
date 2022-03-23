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
using System.Threading.Tasks;

using CiccioSoft.Inventory.Uwp.Models;
using CiccioSoft.Inventory.Uwp.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.Messaging;

namespace CiccioSoft.Inventory.Uwp.ViewModels
{
    public class ProductsViewModel : ViewModelBase
    {
        private readonly ILogger<ProductsViewModel> logger;
        private readonly IProductService productService;

        public ProductsViewModel(ILogger<ProductsViewModel> logger,
                                 IProductService productService,
                                 ProductListViewModel productListViewModel,
                                 ProductDetailsViewModel productDetailsViewModel)
            : base()
        {
            this.logger = logger;
            this.productService = productService;

            ProductList = productListViewModel;
            ProductDetails = productDetailsViewModel;
        }

        public ProductListViewModel ProductList { get; set; }
        public ProductDetailsViewModel ProductDetails { get; set; }

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
            Messenger.Register<ItemMessage<ProductModel>>(this, OnProductMessage);
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


        private async void OnProductMessage(object recipient, ItemMessage<ProductModel> message)
        {
        //    throw new NotImplementedException();
        //}
        //private async void OnMessage(ProductListViewModel viewModel, string message, object args)
        //{
            if (/*recipient == ProductList &&*/ message.Message == "ItemSelected")
            {
                //await ContextService.RunAsync(() =>
                //{
                    await OnItemSelected();
                //});
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
            ProductDetails.Item = selected;
        }

        private async Task PopulateDetails(ProductModel selected)
        {
            try
            {
                var model = await productService.GetProductAsync(selected.ProductID);
                selected.Merge(model);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Load Details");
            }
        }
    }
}
