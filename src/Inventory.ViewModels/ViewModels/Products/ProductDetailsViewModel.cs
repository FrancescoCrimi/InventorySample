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
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

using Inventory.Models;
using Inventory.Services;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Inventory.ViewModels
{
    public class ProductDetailsViewModel : ObservableRecipient // GenericDetailsViewModel<ProductModel>
    {
        private readonly ILogger<ProductDetailsViewModel> logger;
        private readonly IMessageService messageService;
        private readonly IContextService contextService;
        private readonly IProductService productService;
        private readonly IDialogService dialogService;
        private readonly IFilePickerService filePickerService;
        private readonly INavigationService navigationService;

        public ProductDetailsViewModel(ILogger<ProductDetailsViewModel> logger,
                                       IMessageService messageService,
                                       IContextService contextService,
                                       IProductService productService,
                                       IDialogService dialogService,
                                       IFilePickerService filePickerService,
                                       INavigationService navigationService)
        {
            this.logger = logger;
            this.messageService = messageService;
            this.contextService = contextService;
            this.productService = productService;
            this.dialogService = dialogService;
            this.filePickerService = filePickerService;
            this.navigationService = navigationService;
        }

        public ILookupTables LookupTables => LookupTablesProxy.Instance;

        public bool CanGoBack => !contextService.IsMainView && navigationService.CanGoBack;

        public ICommand BackCommand => new RelayCommand(OnBack);
        virtual protected void OnBack()
        {
            //StatusReady();
            messageService.Send(this, "StatusMessage", "Ready");
            if (navigationService.CanGoBack)
            {
                navigationService.GoBack();
            }
        }

        public ICommand EditCommand => new RelayCommand(OnEdit);
        virtual protected void OnEdit()
        {
            //StatusReady();
            messageService.Send(this, "StatusMessage", "Ready");
            BeginEdit();
            messageService.Send(this, "BeginEdit", Item);
        }

        public ICommand DeleteCommand => new RelayCommand(OnDelete);
        virtual protected async void OnDelete()
        {
            //StatusReady();
            messageService.Send(this, "StatusMessage", "Ready");
            if (await ConfirmDeleteAsync())
            {
                await DeleteAsync();
            }
        }
        virtual public async Task DeleteAsync()
        {
            var model = Item;
            if (model != null)
            {
                IsEnabled = false;
                if (await DeleteItemAsync(model))
                {
                    messageService.Send(this, "ItemDeleted", model);
                }
                else
                {
                    IsEnabled = true;
                }
            }
        }

        public ICommand SaveCommand => new RelayCommand(OnSave);
        virtual protected async void OnSave()
        {
            //StatusReady();
            messageService.Send(this, "StatusMessage", "Ready");
            var result = Validate(EditableItem);
            if (result.IsOk)
            {
                await SaveAsync();
            }
            else
            {
                await dialogService.ShowAsync(result.Message, $"{result.Description} Please, correct the error and try again.");
            }
        }
        virtual public async Task SaveAsync()
        {
            IsEnabled = false;
            bool isNew = ItemIsNew;
            if (await SaveItemAsync(EditableItem))
            {
                Item.Merge(EditableItem);
                Item.NotifyChanges();
                OnPropertyChanged(nameof(Title));
                EditableItem = Item;

                if (isNew)
                {
                    messageService.Send(this, "NewItemSaved", Item);
                }
                else
                {
                    messageService.Send(this, "ItemChanged", Item);
                }
                IsEditMode = false;

                OnPropertyChanged(nameof(ItemIsNew));
            }
            IsEnabled = true;
        }
        virtual public Result Validate(ProductModel model)
        {
            foreach (var constraint in GetValidationConstraints(model))
            {
                if (!constraint.Validate(model))
                {
                    return Result.Error("Validation Error", constraint.Message);
                }
            }
            return Result.Ok();
        }

        public ICommand CancelCommand => new RelayCommand(OnCancel);
        virtual protected void OnCancel()
        {
            //StatusReady();
            messageService.Send(this, "StatusMessage", "Ready");
            CancelEdit();
            messageService.Send(this, "CancelEdit", Item);
        }







        public string Title => (Item?.IsNew ?? true) ? "New Product" : TitleEdit;
        public string TitleEdit => Item == null ? "Product" : $"{Item.Name}";

        public bool ItemIsNew => Item?.IsNew ?? true;

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
                    //LogException("Product", "Load", ex);
                    logger.LogCritical(ex, "Load");
                }
            }
        }
        public void Unload()
        {
            ViewModelArgs.ProductID = Item?.ProductID;
        }

        public void Subscribe()
        {
            messageService.Subscribe<ProductDetailsViewModel, ProductModel>(this, OnDetailsMessage);
            messageService.Subscribe<ProductListViewModel>(this, OnListMessage);
        }
        public void Unsubscribe()
        {
            messageService.Unsubscribe(this);
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

        public void BeginEdit()
        {
            NewPictureSource = null;
            if (!IsEditMode)
            {
                IsEditMode = true;
                // Create a copy for edit
                var editableItem = new ProductModel();
                editableItem.Merge(Item);
                EditableItem = editableItem;
            }
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

        protected async Task<bool> SaveItemAsync(ProductModel model)
        {
            //try
            //{
            //    StartStatusMessage("Saving product...");
            await Task.Delay(100);
            await productService.UpdateProductAsync(model);
            //EndStatusMessage("Product saved");

            //LogInformation("Product", "Save", "Product saved successfully", $"Product {model.ProductID} '{model.Name}' was saved successfully.");
            logger.LogInformation($"Product {model.ProductID} '{model.Name}' was saved successfully.");

            return true;
            //}
            //catch (Exception ex)
            //{
            //    StatusError($"Error saving Product: {ex.Message}");
            //    LogException("Product", "Save", ex);
            //    return false;
            //}
        }

        protected async Task<bool> DeleteItemAsync(ProductModel model)
        {
            //try
            //{
            //    StartStatusMessage("Deleting product...");
            await Task.Delay(100);
            await productService.DeleteProductAsync(model);
            //EndStatusMessage("Product deleted");

            //LogWarning("Product", "Delete", "Product deleted", $"Product {model.ProductID} '{model.Name}' was deleted.");
            logger.LogWarning("Product deleted", $"Product {model.ProductID} '{model.Name}' was deleted.");

            return true;
            //}
            //catch (Exception ex)
            //{
            //    StatusError($"Error deleting Product: {ex.Message}");
            //    LogException("Product", "Delete", ex);
            //    return false;
            //}
        }

        protected async Task<bool> ConfirmDeleteAsync()
        {
            return await dialogService.ShowAsync("Confirm Delete", "Are you sure you want to delete current product?", "Ok", "Cancel");
        }

        protected IEnumerable<IValidationConstraint<ProductModel>> GetValidationConstraints(ProductModel model)
        {
            yield return new RequiredConstraint<ProductModel>("Name", m => m.Name);
            yield return new RequiredGreaterThanZeroConstraint<ProductModel>("Category", m => m.CategoryID);
        }

        /*
         *  Handle external messages
         ****************************************************************/
        private async void OnDetailsMessage(ProductDetailsViewModel sender, string message, ProductModel changed)
        {
            var current = Item;
            if (current != null)
            {
                if (changed != null && changed.ProductID == current?.ProductID)
                {
                    switch (message)
                    {
                        case "ItemChanged":
                            await contextService.RunAsync(async () =>
                            {
                                try
                                {
                                    var item = await productService.GetProductAsync(current.ProductID);
                                    item = item ?? new ProductModel { ProductID = current.ProductID, IsEmpty = true };
                                    current.Merge(item);
                                    current.NotifyChanges();
                                    OnPropertyChanged(nameof(Title));
                                    if (IsEditMode)
                                    {
                                        //StatusMessage("WARNING: This product has been modified externally");
                                        messageService.Send(this, "StatusMessage", "WARNING: This product has been modified externally");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //LogException("Product", "Handle Changes", ex);
                                    logger.LogCritical(ex, "Handle Changes");
                                }
                            });
                            break;
                        case "ItemDeleted":
                            await OnItemDeletedExternally();
                            break;
                    }
                }
            }
        }

        private async void OnListMessage(ProductListViewModel sender, string message, object args)
        {
            var current = Item;
            if (current != null)
            {
                switch (message)
                {
                    case "ItemsDeleted":
                        if (args is IList<ProductModel> deletedModels)
                        {
                            if (deletedModels.Any(r => r.ProductID == current.ProductID))
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        break;
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
                            //LogException("Product", "Handle Ranges Deleted", ex);
                            logger.LogCritical(ex, "Handle Ranges Deleted");
                        }
                        break;
                }
            }
        }

        private async Task OnItemDeletedExternally()
        {
            await contextService.RunAsync(() =>
            {
                CancelEdit();
                IsEnabled = false;
                //StatusMessage("WARNING: This product has been deleted externally");
                messageService.Send(this, "StatusMessage", "WARNING: This product has been deleted externally");
            });
        }






        private ProductModel _item = null;
        public ProductModel Item
        {
            get => _item;
            set
            {
                if (SetProperty(ref _item, value))
                {
                    EditableItem = _item;
                    IsEnabled = (!_item?.IsEmpty) ?? false;
                    OnPropertyChanged(nameof(IsDataAvailable));
                    OnPropertyChanged(nameof(IsDataUnavailable));
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        private ProductModel _editableItem = null;
        public ProductModel EditableItem
        {
            get => _editableItem;
            set => SetProperty(ref _editableItem, value);
        }

        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        public bool IsDataAvailable => _item != null;
        public bool IsDataUnavailable => !IsDataAvailable;

        virtual public void CancelEdit()
        {
            if (ItemIsNew)
            {
                // We were creating a new item: cancel means exit
                if (navigationService.CanGoBack)
                {
                    navigationService.GoBack();
                }
                else
                {
                    navigationService.CloseViewAsync();
                }
                return;
            }

            // We were editing an existing item: just cancel edition
            if (IsEditMode)
            {
                EditableItem = Item;
            }
            IsEditMode = false;
        }

        private bool _isEditMode = false;
        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }




    }
}
