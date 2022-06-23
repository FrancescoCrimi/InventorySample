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

using CiccioSoft.Inventory.Data.Models;
using CiccioSoft.Inventory.Uwp.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CiccioSoft.Inventory.Uwp.ViewModels
{
    #region ProductDetailsArgs
    public class ProductDetailsArgs
    {
        static public ProductDetailsArgs CreateDefault() => new ProductDetailsArgs();

        public string ProductID { get; set; }

        public bool IsNew => String.IsNullOrEmpty(ProductID);
    }
    #endregion

    public class ProductDetailsViewModel : GenericDetailsViewModel<ProductModel>
    {
        private readonly ILogger<ProductDetailsViewModel> logger;
        private readonly ProductServiceUwp productService;
        private readonly IFilePickerService filePickerService;
        private readonly IDialogService dialogService;

        public ProductDetailsViewModel(ILogger<ProductDetailsViewModel> logger,
                                       ProductServiceUwp productService,
                                       IFilePickerService filePickerService,
                                       IDialogService dialogService)
            : base()
        {
            this.logger = logger;
            this.productService = productService;
            this.filePickerService = filePickerService;
            this.dialogService = dialogService;
        }

        override public string Title => (Item?.IsNew ?? true) ? "New Product" : TitleEdit;
        public string TitleEdit => Item == null ? "Product" : $"{Item.Name}";

        public override bool ItemIsNew => Item?.IsNew ?? true;

        public ProductDetailsArgs ViewModelArgs { get; private set; }

        public async Task LoadAsync(ProductDetailsArgs args)
        {
            ViewModelArgs = args ?? ProductDetailsArgs.CreateDefault();

            if (ViewModelArgs.IsNew)
            {
                Item = new ProductModel();
                IsEditMode = true;
            }
            else
            {
                try
                {
                    var item = await productService.GetProductAsync(ViewModelArgs.ProductID);
                    Item = item ?? new ProductModel { ProductID = ViewModelArgs.ProductID, IsEmpty = true };
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Load");
                }
            }
        }
        public void Unload()
        {
            ViewModelArgs.ProductID = Item?.ProductID;
        }

        public void Subscribe()
        {
            //MessageService.Subscribe<ProductDetailsViewModel, ProductModel>(this, OnDetailsMessage);
            Messenger.Register<ItemMessage<ProductModel>>(this, OnProductMessage);

            //MessageService.Subscribe<ProductListViewModel>(this, OnListMessage);
            Messenger.Register<ItemMessage<IList<ProductModel>>>(this, OnProductListMessage);
            Messenger.Register<ItemMessage<IList<IndexRange>>>(this, OnIndexRangeListMessage);
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
                ProductID = Item?.ProductID
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
            var result = await filePickerService.OpenImagePickerAsync();
            if (result != null)
            {
                EditableItem.Picture = result.ImageBytes;
                EditableItem.PictureSource = result.ImageSource;
                EditableItem.Thumbnail = result.ImageBytes;
                EditableItem.ThumbnailSource = result.ImageSource;
                NewPictureSource = result.ImageSource;
            }
            else
            {
                NewPictureSource = null;
            }
        }

        protected override async Task<bool> SaveItemAsync(ProductModel model)
        {
            try
            {
                StartStatusMessage("Saving product...");
                await Task.Delay(100);
                await productService.UpdateProductAsync(model);
                EndStatusMessage("Product saved");
                logger.LogInformation($"Product {model.ProductID} '{model.Name}' was saved successfully.");
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Error saving Product: {ex.Message}");
                logger.LogError(ex, "Save");
                return false;
            }
        }

        protected override async Task<bool> DeleteItemAsync(ProductModel model)
        {
            try
            {
                StartStatusMessage("Deleting product...");
                await Task.Delay(100);
                await productService.DeleteProductAsync(model);
                EndStatusMessage("Product deleted");
                logger.LogWarning($"Product {model.ProductID} '{model.Name}' was deleted.");
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Error deleting Product: {ex.Message}");
                logger.LogError(ex, "Delete");
                return false;
            }
        }

        protected override async Task<bool> ConfirmDeleteAsync()
        {
            return await dialogService.ShowAsync("Confirm Delete", "Are you sure you want to delete current product?", "Ok", "Cancel");
        }

        override protected IEnumerable<IValidationConstraint<ProductModel>> GetValidationConstraints(ProductModel model)
        {
            yield return new RequiredConstraint<ProductModel>("Name", m => m.Name);
            yield return new RequiredGreaterThanZeroConstraint<ProductModel>("Category", m => m.CategoryID);
        }

        /*
         *  Handle external messages
         ****************************************************************/

        private async void OnProductMessage(object recipient, ItemMessage<ProductModel> message)
        {
        //    throw new NotImplementedException();
        //}
        //private async void OnDetailsMessage(ProductDetailsViewModel sender, string message, ProductModel changed)
        //{
            var current = Item;
            if (current != null)
            {
                if (message.Value != null && message.Value.ProductID == current?.ProductID)
                {
                    switch (message.Message)
                    {
                        case "ItemChanged":
                            //await ContextService.RunAsync(async () =>
                            //{
                                try
                                {
                                    var item = await productService.GetProductAsync(current.ProductID);
                                    item = item ?? new ProductModel { ProductID = current.ProductID, IsEmpty = true };
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
                                    logger.LogError(ex, "Handle Changes");
                                }
                            //});
                            break;
                        case "ItemDeleted":
                            await OnItemDeletedExternally();
                            break;
                    }
                }
            }
        }


        private async void OnIndexRangeListMessage(object recipient, ItemMessage<IList<IndexRange>> message)
        {
            var current = Item;
            if (current != null)
            {
                switch (message.Message)
                {
                    case "ItemRangesDeleted":
                        try
                        {
                            var model = await productService.GetProductAsync(current.ProductID);
                            if (model == null)
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Handle Ranges Deleted");
                        }
                        break;
                }
            }
        }

        private async void OnProductListMessage(object recipient, ItemMessage<IList<ProductModel>> message)
        {
            var current = Item;
            if (current != null)
            {
                switch (message.Message)
                {
                    case "ItemsDeleted":
                        //if (args is IList<ProductModel> deletedModels)
                        //{
                            if (message.Value.Any(r => r.ProductID == current.ProductID))
                            {
                                await OnItemDeletedExternally();
                            }
                        //}
                        break;
                }
            }
        }
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
        //                    var model = await productService.GetProductAsync(current.ProductID);
        //                    if (model == null)
        //                    {
        //                        await OnItemDeletedExternally();
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    logger.LogError(ex, "Handle Ranges Deleted");
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
    }
}
