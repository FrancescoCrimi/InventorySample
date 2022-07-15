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

using CiccioSoft.Inventory.Application.Models;
using CiccioSoft.Inventory.Uwp.Services;
using CiccioSoft.Inventory.Uwp.Services.Infrastructure;
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
    #region CustomerDetailsArgs
    public class CustomerDetailsArgs
    {
        static public CustomerDetailsArgs CreateDefault() => new CustomerDetailsArgs();

        public long CustomerID { get; set; }

        public bool IsNew => CustomerID <= 0;
    }
    #endregion

    public class CustomerDetailsViewModel : GenericDetailsViewModel<CustomerModel>
    {
        private readonly ILogger<CustomerDetailsViewModel> logger;
        private readonly CustomerServiceUwp customerService;
        private readonly FilePickerService filePickerService;

        public CustomerDetailsViewModel(ILogger<CustomerDetailsViewModel> logger,
                                        CustomerServiceUwp customerService,
                                        FilePickerService filePickerService)
            : base()
        {
            this.logger = logger;
            this.customerService = customerService;
            this.filePickerService = filePickerService;
        }


        override public string Title => (Item?.IsNew ?? true) ? "New Customer" : TitleEdit;
        public string TitleEdit => Item == null ? "Customer" : $"{Item.FullName}";

        public override bool ItemIsNew => Item?.IsNew ?? true;

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
                    logger.LogError(ex, "Load");
                }
            }
        }
        public void Unload()
        {
            ViewModelArgs.CustomerID = Item?.CustomerID ?? 0;
        }

        public void Subscribe()
        {
            //MessageService.Subscribe<CustomerDetailsViewModel, CustomerModel>(this, OnDetailsMessage);
            Messenger.Register<ItemMessage<CustomerModel>>(this, OnCustomerMessage);

            //MessageService.Subscribe<CustomerListViewModel>(this, OnListMessage);
            Messenger.Register<ItemMessage<IList<CustomerModel>>>(this, OnCustomerListMessage);
            Messenger.Register<ItemMessage<IList<IndexRange>>>(this, OnIndexRangeListMessage);
        }

        public void Unsubscribe()
        {
            //MessageService.Unsubscribe(this);
            Messenger.UnregisterAll(this);
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

        protected override async Task<bool> SaveItemAsync(CustomerModel model)
        {
            try
            {
                StartStatusMessage("Saving customer...");
                await Task.Delay(100);
                await customerService.UpdateCustomerAsync(model);
                EndStatusMessage("Customer saved");
                logger.LogInformation($"Customer {model.CustomerID} '{model.FullName}' was saved successfully.");
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Error saving Customer: {ex.Message}");
                logger.LogError(ex, "Save");
                return false;
            }
        }

        protected override async Task<bool> DeleteItemAsync(CustomerModel model)
        {
            try
            {
                StartStatusMessage("Deleting customer...");
                await Task.Delay(100);
                await customerService.DeleteCustomerAsync(model);
                EndStatusMessage("Customer deleted");
                logger.LogWarning($"Customer {model.CustomerID} '{model.FullName}' was deleted.");
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Error deleting Customer: {ex.Message}");
                logger.LogError(ex, "Delete");
                return false;
            }
        }

        protected override async Task<bool> ConfirmDeleteAsync()
        {
            return await ShowDialogAsync("Confirm Delete", "Are you sure you want to delete current customer?", "Ok", "Cancel");
        }

        override protected IEnumerable<IValidationConstraint<CustomerModel>> GetValidationConstraints(CustomerModel model)
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

        private async void OnCustomerMessage(object recipient, ItemMessage<CustomerModel> message)
        {
            //    throw new NotImplementedException();
            //}
            //private async void OnDetailsMessage(CustomerDetailsViewModel sender, string message, CustomerModel changed)
            //{
            var current = Item;
            if (current != null)
            {
                if (message.Value != null && message.Value.CustomerID == current?.CustomerID)
                {
                    switch (message.Message)
                    {
                        case "ItemChanged":
                            //await ContextService.RunAsync(async () =>
                            //{
                            try
                            {
                                var item = await customerService.GetCustomerAsync(current.CustomerID);
                                item = item ?? new CustomerModel { CustomerID = current.CustomerID, IsEmpty = true };
                                current.Merge(item);
                                current.NotifyChanges();
                                OnPropertyChanged(nameof(Title));
                                if (IsEditMode)
                                {
                                    StatusMessage("WARNING: This customer has been modified externally");
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

        private async void OnCustomerListMessage(object recipient, ItemMessage<IList<CustomerModel>> message)
        {
            var current = Item;
            if (current != null)
            {
                switch (message.Message)
                {
                    case "ItemsDeleted":
                        if (message.Value.Any(r => r.CustomerID == current.CustomerID))
                        {
                            await OnItemDeletedExternally();
                        }
                        break;
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
                            var model = await customerService.GetCustomerAsync(current.CustomerID);
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

        //private async void OnListMessage(CustomerListViewModel sender, string message, object args)
        //{
        //    var current = Item;
        //    if (current != null)
        //    {
        //        switch (message)
        //        {
        //            case "ItemsDeleted":
        //                if (args is IList<CustomerModel> deletedModels)
        //                {
        //                    if (deletedModels.Any(r => r.CustomerID == current.CustomerID))
        //                    {
        //                        await OnItemDeletedExternally();
        //                    }
        //                }
        //                break;
        //            case "ItemRangesDeleted":
        //                try
        //                {
        //                    var model = await customerService.GetCustomerAsync(current.CustomerID);
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
                StatusMessage("WARNING: This customer has been deleted externally");
            });
            //});
        }
    }
}
