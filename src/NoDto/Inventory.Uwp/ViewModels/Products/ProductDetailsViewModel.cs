// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Inventory.Application;
using Inventory.Domain.Aggregates.ProductAggregate;
using Inventory.Infrastructure.Logging;
using Inventory.Uwp.Common;
using Inventory.Uwp.Services;
using Inventory.Uwp.ViewModels.Common;
using Inventory.Uwp.ViewModels.Message;
using Microsoft.Extensions.Logging;

namespace Inventory.Uwp.ViewModels.Products
{
    public class ProductDetailsViewModel : GenericDetailsViewModel<Product>
    {
        private readonly ILogger _logger;
        private readonly ProductService _productService;
        private readonly FilePickerService _filePickerService;

        public ProductDetailsViewModel(ILogger<ProductDetailsViewModel> logger,
                                       NavigationService navigationService,
                                       WindowManagerService windowService,
                                       ProductService productService,
                                       FilePickerService filePickerService)
            : base(navigationService, windowService)
        {
            _logger = logger;
            _productService = productService;
            _filePickerService = filePickerService;
        }

        #region property

        public override string Title => Item?.IsNew ?? true ? "New Product" : TitleEdit;

        public string TitleEdit => Item == null ? "Product" : $"{Item.Name}";

        public override bool ItemIsNew => Item?.IsNew ?? true;

        public ProductDetailsArgs ViewModelArgs
        {
            get; private set;
        }

        private object _newPictureSource = null;
        public object NewPictureSource
        {
            get => _newPictureSource;
            set => SetProperty(ref _newPictureSource, value);
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



        public IEnumerable<Category> Categories => _productService.Categories;

        #endregion


        #region method

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
                    var item = await _productService.GetProductAsync(ViewModelArgs.ProductId);
                    Item = item ?? new Product { Id = ViewModelArgs.ProductId, IsEmpty = true };
                }
                catch (Exception ex)
                {
                    _logger.LogError(LogEvents.Load, ex, "Load Product");
                }
            }
        }

        public void Unload()
        {
            ViewModelArgs.ProductId = Item.Id;
        }

        public void Subscribe()
        {
            //MessageService.Subscribe<ProductDetailsViewModel, ProductModel>(this, OnDetailsMessage);
            //MessageService.Subscribe<ProductListViewModel>(this, OnListMessage);
            Messenger.Register<ViewModelsMessage<Product>>(this, OnMessage);
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
                ProductId = Item.Id
            };
        }

        public override void BeginEdit()
        {
            NewPictureSource = null;
            base.BeginEdit();
        }

        #endregion


        #region protected and private method

        protected async override Task<bool> SaveItemAsync(Product model)
        {
            try
            {
                StartStatusMessage("Saving product...");
                await Task.Delay(100);
                await _productService.UpdateProductAsync(model);
                EndStatusMessage("Product saved");
                _logger.LogInformation(LogEvents.Save, $"Product {model.Id} '{model.Name}' was saved successfully.");
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Error saving Product: {ex.Message}");
                _logger.LogError(LogEvents.Save, ex, "Error saving Product");
                return false;
            }
        }

        protected async override Task<bool> DeleteItemAsync(Product model)
        {
            try
            {
                StartStatusMessage("Deleting product...");
                await Task.Delay(100);
                await _productService.DeleteProductsAsync(model);
                EndStatusMessage("Product deleted");
                _logger.LogWarning(LogEvents.Delete, $"Product {model.Id} '{model.Name}' was deleted.");
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Error deleting Product: {ex.Message}");
                _logger.LogError(LogEvents.Delete, ex, "Error deleting Product");
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


        private async void OnMessage(object recipient, ViewModelsMessage<Product> message)
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
                                //var item = await _productRepository.GetProductAsync(current.Id);
                                //item = item ?? new Product { Id = current.Id, IsEmpty = true };
                                //current.Merge(item);
                                //current.NotifyChanges();
                                Item = await GetItemAsync(current.Id);
                                OnPropertyChanged(nameof(Title));
                                if (IsEditMode)
                                {
                                    StatusMessage("WARNING: This product has been modified externally");
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(LogEvents.HandleChanges, ex, "Handle Product Changes");
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
                            _logger.LogError(LogEvents.HandleRangesDeleted, ex, "Handle Product Ranges Deleted");
                        }
                        break;
                }
            }
        }

        private async Task OnItemDeletedExternally()
        {
            await Task.Run(() =>
            {
                CancelEdit();
                IsEnabled = false;
                StatusMessage("WARNING: This product has been deleted externally");
            });
        }

        protected async override Task<Product> GetItemAsync(long id)
        {
            return await _productService.GetProductAsync(id);
        }

        #endregion
    }
}
