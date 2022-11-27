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

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Inventory.Uwp.Library.Common;
using Inventory.Uwp.ViewModels;
using Inventory.Uwp.Common;
using Inventory.Uwp.Dto;
using Inventory.Uwp.Services;
using Inventory.Uwp.ViewModels.Common;

namespace Inventory.Uwp.ViewModels.Customers
{
    #region CustomerDetailsArgs
    public class CustomerDetailsArgs
    {
        public static CustomerDetailsArgs CreateDefault() => new CustomerDetailsArgs();

        public long CustomerID { get; set; }

        public bool IsNew => CustomerID <= 0;
    }
    #endregion

    public class CustomerDetailsViewModel : GenericDetailsViewModel<CustomerDto>
    {
        private readonly ILogger<CustomerDetailsViewModel> logger;
        private readonly CustomerServiceFacade customerService;
        private readonly FilePickerService filePickerService;

        public CustomerDetailsViewModel(ILogger<CustomerDetailsViewModel> logger,
                                        CustomerServiceFacade customerService,
                                        FilePickerService filePickerService)
            : base()
        {
            this.logger = logger;
            this.customerService = customerService;
            this.filePickerService = filePickerService;
        }


        public override string Title => Item?.IsNew ?? true ? "New Customer" : TitleEdit;
        public string TitleEdit => Item == null ? "Customer" : $"{Item.FullName}";

        public override bool ItemIsNew => Item?.IsNew ?? true;

        public CustomerDetailsArgs ViewModelArgs { get; private set; }

        public async Task LoadAsync(CustomerDetailsArgs args)
        {
            ViewModelArgs = args ?? CustomerDetailsArgs.CreateDefault();

            if (ViewModelArgs.IsNew)
            {
                Item = new CustomerDto();
                IsEditMode = true;
            }
            else
            {
                try
                {
                    var item = await customerService.GetCustomerAsync(ViewModelArgs.CustomerID);
                    Item = item ?? new CustomerDto { CustomerID = ViewModelArgs.CustomerID, IsEmpty = true };
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
            Messenger.Register<ItemMessage<CustomerDto>>(this, OnCustomerMessage);

            //MessageService.Subscribe<CustomerListViewModel>(this, OnListMessage);
            Messenger.Register<ItemMessage<IList<CustomerDto>>>(this, OnCustomerListMessage);
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

        protected override async Task<bool> SaveItemAsync(CustomerDto model)
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

        protected override async Task<bool> DeleteItemAsync(CustomerDto model)
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

        protected override IEnumerable<IValidationConstraint<CustomerDto>> GetValidationConstraints(CustomerDto model)
        {
            yield return new RequiredConstraint<CustomerDto>("First Name", m => m.FirstName);
            yield return new RequiredConstraint<CustomerDto>("Last Name", m => m.LastName);
            yield return new RequiredConstraint<CustomerDto>("Email Address", m => m.EmailAddress);
            yield return new RequiredConstraint<CustomerDto>("Address Line 1", m => m.AddressLine1);
            yield return new RequiredConstraint<CustomerDto>("City", m => m.City);
            yield return new RequiredConstraint<CustomerDto>("Region", m => m.Region);
            yield return new RequiredConstraint<CustomerDto>("Postal Code", m => m.PostalCode);
            yield return new RequiredConstraint<CustomerDto>("Country", m => m.CountryCode);
        }

        /*
         *  Handle external messages
         ****************************************************************/

        //private async void OnDetailsMessage(CustomerDetailsViewModel sender, string message, CustomerModel changed)
        private async void OnCustomerMessage(object recipient, ItemMessage<CustomerDto> message)
        {
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
                                item = item ?? new CustomerDto { CustomerID = current.CustomerID, IsEmpty = true };
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

        private async void OnCustomerListMessage(object recipient, ItemMessage<IList<CustomerDto>> message)
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