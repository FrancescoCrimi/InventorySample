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
using Inventory.Application;
using Inventory.Domain.Model;
using Inventory.Uwp.Common;
using Inventory.Uwp.Services;
using Inventory.Uwp.ViewModels.Common;
using Inventory.Uwp.ViewModels.Message;
using Microsoft.Extensions.Logging;

namespace Inventory.Uwp.ViewModels.Products
{
    public class ProductDetailsViewModel : GenericDetailsViewModel<Product>
    {
        private readonly ILogger<ProductDetailsViewModel> _logger;
        private readonly IProductService _productService;
        private readonly FilePickerService _filePickerService;

        public ProductDetailsViewModel(ILogger<ProductDetailsViewModel> logger,
                                       IProductService productService,
                                       FilePickerService filePickerService)
            : base()
        {
            _logger = logger;
            _productService = productService;
            _filePickerService = filePickerService;
        }

        public override string Title => Item?.IsNew ?? true ? "New Product" : TitleEdit;
        public string TitleEdit => Item == null ? "Product" : $"{Item.Name}";

        public override bool ItemIsNew => Item?.IsNew ?? true;

        public ProductDetailsArgs ViewModelArgs
        {
            get; private set;
        }

        public async Task LoadAsync(ProductDetailsArgs args)
        {
            ViewModelArgs = args ?? ProductDetailsArgs.CreateDefault();

            if (ViewModelArgs.IsNew)
            {
                Item = new Product();
                IsEditMode = true;
            }
            else
            {
                try
                {
                    var item = await _productService.GetProductAsync(ViewModelArgs.ProductID);
                    Item = item ?? new Product { Id = ViewModelArgs.ProductID, IsEmpty = true };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Load");
                }
            }
        }
        public void Unload()
        {
            ViewModelArgs.ProductID = Item.Id;
        }

        public void Subscribe()
        {
            //MessageService.Subscribe<ProductDetailsViewModel, ProductModel>(this, OnDetailsMessage);
            //MessageService.Subscribe<ProductListViewModel>(this, OnListMessage);
            Messenger.Register<ProductChangeMessage>(this, OnMessage);
        }

        public void Unsubscribe()
        {
            //MessageService.Unsubscribe(this);
            Messenger.UnregisterAll(this);
        }

        public ProductDetailsArgs CreateArgs()
        {
            return new ProductDetailsArgs
            {
                ProductID = Item.Id
            };
        }

        private object _newPictureSource = null;
        public object NewPictureSource
        {
            get => _newPictureSource;
            set => SetProperty(ref _newPictureSource, value);
        }

        public override void BeginEdit()
        {
            NewPictureSource = null;
            base.BeginEdit();
        }

        public ICommand EditPictureCommand => new RelayCommand(OnEditPicture);
        private async void OnEditPicture()
        {
            NewPictureSource = null;
            var result = await _filePickerService.OpenImagePickerAsync();
            if (result != null)
            {
                EditableItem.Picture = result.ImageBytes;
                //EditableItem.PictureSource = result.ImageSource;
                EditableItem.Thumbnail = result.ImageBytes;
                //EditableItem.ThumbnailSource = result.ImageSource;
                NewPictureSource = result.ImageSource;
            }
            else
            {
                NewPictureSource = null;
            }
        }

        protected async override Task<bool> SaveItemAsync(Product model)
        {
            try
            {
                StartStatusMessage("Saving product...");
                await Task.Delay(100);
                await _productService.UpdateProductAsync(model);
                EndStatusMessage("Product saved");
                _logger.LogInformation($"Product {model.Id} '{model.Name}' was saved successfully.");
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Error saving Product: {ex.Message}");
                _logger.LogError(ex, "Save");
                return false;
            }
        }

        protected async override Task<bool> DeleteItemAsync(Product model)
        {
            try
            {
                StartStatusMessage("Deleting product...");
                await Task.Delay(100);
                await _productService.DeleteProductAsync(model);
                EndStatusMessage("Product deleted");
                _logger.LogWarning($"Product {model.Id} '{model.Name}' was deleted.");
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Error deleting Product: {ex.Message}");
                _logger.LogError(ex, "Delete");
                return false;
            }
        }

        protected async override Task<bool> ConfirmDeleteAsync()
        {
            return await ShowDialogAsync("Confirm Delete", "Are you sure you want to delete current product?", "Ok", "Cancel");
        }

        protected override IEnumerable<IValidationConstraint<Product>> GetValidationConstraints(Product model)
        {
            yield return new RequiredConstraint<Product>("Name", m => m.Name);
            yield return new RequiredGreaterThanZeroConstraint<Product>("Category", m => m.CategoryId);
        }

        /*
         *  Handle external messages
         ****************************************************************/

        private async void OnMessage(object recipient, ProductChangeMessage message)
        {
            var current = Item;
            if (current != null)
            {
                switch (message.Value)
                {
                    case "ItemChanged":
                        if (message.Id != 0 && message.Id == current?.Id)
                        {
                            try
                            {
                                var item = await _productService.GetProductAsync(current.Id);
                                item = item ?? new Product { Id = current.Id, IsEmpty = true };
                                current.Merge(item);
                                current.NotifyChanges();
                                OnPropertyChanged(nameof(Title));
                                if (IsEditMode)
                                {
                                    StatusMessage("WARNING: This product has been modified externally");
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Handle Changes");
                            }
                        }
                        break;

                    case "ItemDeleted":
                        if (message.Id != 0 && message.Id == current?.Id)
                        {
                            await OnItemDeletedExternally();
                        }
                        break;


                    case "ItemsDeleted":
                        if (message.SelectedItems != null)
                        {
                            if (message.SelectedItems.Any(r => r.Id == current.Id))
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        break;

                    case "ItemRangesDeleted":
                        try
                        {
                            var model = await _productService.GetProductAsync(current.Id);
                            if (model == null)
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Handle Ranges Deleted");
                        }
                        break;
                }
            }
        }


        //private async void OnDetailsMessage(ProductDetailsViewModel sender, string message, ProductModel changed)
        //{
        //    var current = Item;
        //    if (current != null)
        //    {
        //        if (changed != null && changed.ProductID == current?.ProductID)
        //        {
        //            switch (message)
        //            {
        //                case "ItemChanged":
        //                    await ContextService.RunAsync(async () =>
        //                    {
        //                        try
        //                        {
        //                            var item = await ProductService.GetProductAsync(current.ProductID);
        //                            item = item ?? new ProductModel { ProductID = current.ProductID, IsEmpty = true };
        //                            current.Merge(item);
        //                            current.NotifyChanges();
        //                            NotifyPropertyChanged(nameof(Title));
        //                            if (IsEditMode)
        //                            {
        //                                StatusMessage("WARNING: This product has been modified externally");
        //                            }
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            LogException("Product", "Handle Changes", ex);
        //                        }
        //                    });
        //                    break;
        //                case "ItemDeleted":
        //                    await OnItemDeletedExternally();
        //                    break;
        //            }
        //        }
        //    }
        //}

        //private async void OnListMessage(ProductListViewModel sender, string message, object args)
        //{
        //    var current = Item;
        //    if (current != null)
        //    {
        //        switch (message)
        //        {
        //            case "ItemsDeleted":
        //                if (args is IList<ProductModel> deletedModels)
        //                {
        //                    if (deletedModels.Any(r => r.ProductID == current.ProductID))
        //                    {
        //                        await OnItemDeletedExternally();
        //                    }
        //                }
        //                break;
        //            case "ItemRangesDeleted":
        //                try
        //                {
        //                    var model = await ProductService.GetProductAsync(current.ProductID);
        //                    if (model == null)
        //                    {
        //                        await OnItemDeletedExternally();
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    LogException("Product", "Handle Ranges Deleted", ex);
        //                }
        //                break;
        //        }
        //    }
        //}

        private async Task OnItemDeletedExternally()
        {
            //await ContextService.RunAsync(() =>
            //{
            await Task.Run(() =>
            {
                CancelEdit();
                IsEnabled = false;
                StatusMessage("WARNING: This product has been deleted externally");
            });
            //});
        }

        protected override void SendItemChangedMessage(string message, long itemId)
            => Messenger.Send(new ProductChangeMessage(message, itemId));
    }
}
