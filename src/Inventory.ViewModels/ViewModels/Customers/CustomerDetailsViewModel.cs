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
    public class CustomerDetailsViewModel : ObservableRecipient  //GenericDetailsViewModel<CustomerModel>
    {
        private readonly ILogger<CustomerDetailsViewModel> logger;
        private readonly IMessageService messageService;
        private readonly IContextService contextService;
        private readonly IDialogService dialogService;
        private readonly INavigationService navigationService;
        private readonly ICustomerService customerService;
        private readonly IFilePickerService filePickerService;

        public CustomerDetailsViewModel(ILogger<CustomerDetailsViewModel> logger,
                                        ICustomerService customerService,
                                        IMessageService messageService,
                                        IContextService contextService,
                                        IDialogService dialogService,
                                        IFilePickerService filePickerService,
                                        INavigationService navigationService)
        {
            this.logger = logger;
            this.customerService = customerService;
            this.messageService = messageService;
            this.contextService = contextService;
            this.dialogService = dialogService;
            this.filePickerService = filePickerService;
            this.navigationService = navigationService;
        }

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






        public ILookupTables LookupTables => LookupTablesProxy.Instance;

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

        public ICommand CancelCommand => new RelayCommand(OnCancel);
        virtual protected void OnCancel()
        {
            //StatusReady();
            messageService.Send(this, "StatusMessage", "Ready");
            CancelEdit();
            messageService.Send(this, "CancelEdit", Item);
        }


        virtual public Result Validate(CustomerModel model)
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







        private CustomerModel _item = null;
        public CustomerModel Item
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

        private CustomerModel _editableItem = null;
        public CustomerModel EditableItem
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

        public string Title => (Item?.IsNew ?? true) ? "New Customer" : TitleEdit;
        public string TitleEdit => Item == null ? "Customer" : $"{Item.FullName}";

        public bool ItemIsNew => Item?.IsNew ?? true;

        private bool _isEditMode = false;
        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }


        public CustomerDetailsArgs ViewModelArgs { get; private set; }

        public async Task LoadAsync(CustomerDetailsArgs args)
        {
            ViewModelArgs = args ?? CustomerDetailsArgs.CreateDefault();

            if (ViewModelArgs.IsNew)
            {
                Item = new CustomerModel();
                IsEditMode = true;
            }
            else
            {
                try
                {
                    var item = await customerService.GetCustomerAsync(ViewModelArgs.CustomerID);
                    Item = item ?? new CustomerModel { CustomerID = ViewModelArgs.CustomerID, IsEmpty = true };
                }
                catch (Exception ex)
                {
                    //LogException("Customer", "Load", ex);
                    logger.LogCritical(ex, "Load");
                }
            }
        }
        public void Unload()
        {
            ViewModelArgs.CustomerID = Item?.CustomerID ?? 0;
        }

        public void Subscribe()
        {
            messageService.Subscribe<CustomerDetailsViewModel, CustomerModel>(this, OnDetailsMessage);
            messageService.Subscribe<CustomerListViewModel>(this, OnListMessage);
        }
        public void Unsubscribe()
        {
            messageService.Unsubscribe(this);
        }

        public CustomerDetailsArgs CreateArgs()
        {
            return new CustomerDetailsArgs
            {
                CustomerID = Item?.CustomerID ?? 0
            };
        }

        private object _newPictureSource = null;

        public object NewPictureSource
        {
            get => _newPictureSource;
            set => SetProperty(ref _newPictureSource, value);
        }

        public  void BeginEdit()
        {
            NewPictureSource = null;
            if (!IsEditMode)
            {
                IsEditMode = true;
                // Create a copy for edit
                var editableItem = new CustomerModel();
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

        protected async Task<bool> SaveItemAsync(CustomerModel model)
        {
            //try
            //{
            //    StartStatusMessage("Saving customer...");
                await Task.Delay(100);
                await customerService.UpdateCustomerAsync(model);
                //EndStatusMessage("Customer saved");

                //LogInformation("Customer", "Save", "Customer saved successfully", $"Customer {model.CustomerID} '{model.FullName}' was saved successfully.");
            logger.LogInformation("Customer saved successfully", $"Customer {model.CustomerID} '{model.FullName}' was saved successfully.");

            return true;
            //}
            //catch (Exception ex)
            //{
            //    StatusError($"Error saving Customer: {ex.Message}");
            //    LogException("Customer", "Save", ex);
            //    return false;
            //}
        }

        protected async Task<bool> DeleteItemAsync(CustomerModel model)
        {
            //try
            //{
            //    StartStatusMessage("Deleting customer...");
                await Task.Delay(100);
                await customerService.DeleteCustomerAsync(model);
                //EndStatusMessage("Customer deleted");

                //LogWarning("Customer", "Delete", "Customer deleted", $"Customer {model.CustomerID} '{model.FullName}' was deleted.");
            logger.LogWarning("Customer deleted", $"Customer {model.CustomerID} '{model.FullName}' was deleted.");

                return true;
            //}
            //catch (Exception ex)
            //{
            //    StatusError($"Error deleting Customer: {ex.Message}");
            //    LogException("Customer", "Delete", ex);
            //    return false;
            //}
        }

        protected async Task<bool> ConfirmDeleteAsync()
        {
            return await dialogService.ShowAsync("Confirm Delete", "Are you sure you want to delete current customer?", "Ok", "Cancel");
        }

         protected IEnumerable<IValidationConstraint<CustomerModel>> GetValidationConstraints(CustomerModel model)
        {
            yield return new RequiredConstraint<CustomerModel>("First Name", m => m.FirstName);
            yield return new RequiredConstraint<CustomerModel>("Last Name", m => m.LastName);
            yield return new RequiredConstraint<CustomerModel>("Email Address", m => m.EmailAddress);
            yield return new RequiredConstraint<CustomerModel>("Address Line 1", m => m.AddressLine1);
            yield return new RequiredConstraint<CustomerModel>("City", m => m.City);
            yield return new RequiredConstraint<CustomerModel>("Region", m => m.Region);
            yield return new RequiredConstraint<CustomerModel>("Postal Code", m => m.PostalCode);
            yield return new RequiredConstraint<CustomerModel>("Country", m => m.CountryCode);
        }

        /*
         *  Handle external messages
         ****************************************************************/
        private async void OnDetailsMessage(CustomerDetailsViewModel sender, string message, CustomerModel changed)
        {
            var current = Item;
            if (current != null)
            {
                if (changed != null && changed.CustomerID == current?.CustomerID)
                {
                    switch (message)
                    {
                        case "ItemChanged":
                            await contextService.RunAsync(async () =>
                            {
                                try
                                {
                                    var item = await customerService.GetCustomerAsync(current.CustomerID);
                                    item = item ?? new CustomerModel { CustomerID = current.CustomerID, IsEmpty = true };
                                    current.Merge(item);
                                    current.NotifyChanges();
                                    OnPropertyChanged(nameof(Title));
                                    if (IsEditMode)
                                    {
                                        //StatusMessage("WARNING: This customer has been modified externally")
                                        messageService.Send(this, "StatusMessage", "WARNING: This customer has been modified externally");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //LogException("Customer", "Handle Changes", ex);
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

        private async void OnListMessage(CustomerListViewModel sender, string message, object args)
        {
            var current = Item;
            if (current != null)
            {
                switch (message)
                {
                    case "ItemsDeleted":
                        if (args is IList<CustomerModel> deletedModels)
                        {
                            if (deletedModels.Any(r => r.CustomerID == current.CustomerID))
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        break;
                    case "ItemRangesDeleted":
                        try
                        {
                            var model = await customerService.GetCustomerAsync(current.CustomerID);
                            if (model == null)
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        catch (Exception ex)
                        {
                            //LogException("Customer", "Handle Ranges Deleted", ex);
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
                //StatusMessage("WARNING: This customer has been deleted externally");
                messageService.Send(this, "StatusMessage", "WARNING: This customer has been deleted externally");
            });
        }

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
    }
}
