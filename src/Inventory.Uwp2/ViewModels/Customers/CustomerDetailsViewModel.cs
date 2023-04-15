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

namespace Inventory.Uwp.ViewModels.Customers
{
    public class CustomerDetailsViewModel : GenericDetailsViewModel<Customer>
    {
        private readonly ILogger<CustomerDetailsViewModel> _logger;
        private readonly CustomerService _customerService;
        private readonly FilePickerService _filePickerService;

        public CustomerDetailsViewModel(ILogger<CustomerDetailsViewModel> logger,
                                        CustomerService customerService,
                                        FilePickerService filePickerService)
            : base()
        {
            _logger = logger;
            _customerService = customerService;
            _filePickerService = filePickerService;
        }


        public override string Title => Item?.IsNew ?? true ? "New Customer" : TitleEdit;
        public string TitleEdit => Item == null ? "Customer" : $"{Item.FullName}";

        public override bool ItemIsNew => Item?.IsNew ?? true;

        public CustomerDetailsArgs ViewModelArgs
        {
            get; private set;
        }

        public async Task LoadAsync(CustomerDetailsArgs args)
        {
            ViewModelArgs = args ?? CustomerDetailsArgs.CreateDefault();

            if (ViewModelArgs.IsNew)
            {
                Item = new Customer();
                IsEditMode = true;
            }
            else
            {
                try
                {
                    var item = await _customerService.GetCustomerAsync(ViewModelArgs.CustomerID);
                    Item = item ?? new Customer { Id = ViewModelArgs.CustomerID, IsEmpty = true };
                    OnPropertyChanged(nameof(Item));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Load");
                }
            }
        }
        public void Unload()
        {
            ViewModelArgs.CustomerID = Item?.Id ?? 0;
        }

        public void Subscribe()
        {
            //MessageService.Subscribe<CustomerDetailsViewModel, CustomerModel>(this, OnDetailsMessage);
            //MessageService.Subscribe<CustomerListViewModel>(this, OnListMessage);
            Messenger.Register<CustomerChangedMessage>(this, OnMessage);
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
                CustomerID = Item?.Id ?? 0
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

        protected async override Task<bool> SaveItemAsync(Customer model)
        {
            try
            {
                StartStatusMessage("Saving customer...");
                await Task.Delay(100);
                await _customerService.UpdateCustomerAsync(model);
                EndStatusMessage("Customer saved");
                _logger.LogInformation($"Customer {model.Id} '{model.FullName}' was saved successfully.");
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Error saving Customer: {ex.Message}");
                _logger.LogError(ex, "Save");
                return false;
            }
        }

        protected async override Task<bool> DeleteItemAsync(Customer model)
        {
            try
            {
                StartStatusMessage("Deleting customer...");
                await Task.Delay(100);
                await _customerService.DeleteCustomerAsync(model);
                EndStatusMessage("Customer deleted");
                _logger.LogWarning($"Customer {model.Id} '{model.FullName}' was deleted.");
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Error deleting Customer: {ex.Message}");
                _logger.LogError(ex, "Delete");
                return false;
            }
        }

        protected async override Task<bool> ConfirmDeleteAsync()
        {
            return await ShowDialogAsync("Confirm Delete", "Are you sure you want to delete current customer?", "Ok", "Cancel");
        }

        protected override IEnumerable<IValidationConstraint<Customer>> GetValidationConstraints(Customer model)
        {
            yield return new RequiredConstraint<Customer>("First Name", m => m.FirstName);
            yield return new RequiredConstraint<Customer>("Last Name", m => m.LastName);
            yield return new RequiredConstraint<Customer>("Email Address", m => m.EmailAddress);
            yield return new RequiredConstraint<Customer>("Address Line 1", m => m.AddressLine1);
            yield return new RequiredConstraint<Customer>("City", m => m.City);
            yield return new RequiredConstraint<Customer>("Region", m => m.Region);
            yield return new RequiredConstraint<Customer>("Postal Code", m => m.PostalCode);
            yield return new RequiredConstraint<Customer>("Country", m => m.Country);
        }

        /*
         *  Handle external messages
         ****************************************************************/

        private async void OnMessage(object recipient, CustomerChangedMessage message)
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
                                var item = await _customerService.GetCustomerAsync(current.Id);
                                item = item ?? new Customer { Id = current.Id, IsEmpty = true };
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
                                //LogException("Customer", "Handle Changes", ex);
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
                            var model = await _customerService.GetCustomerAsync(current.Id);
                            if (model == null)
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        catch (Exception ex)
                        {
                            //LogException("Customer", "Handle Ranges Deleted", ex);
                            _logger.LogError(ex, "Handle Ranges Deleted");
                        }
                        break;
                }
            }
        }

        //private async void OnDetailsMessage(CustomerDetailsViewModel sender, string message, CustomerModel changed)
        //{
        //    var current = Item;
        //    if (current != null)
        //    {
        //        if (changed != null && changed.CustomerID == current?.CustomerID)
        //        {
        //            switch (message)
        //            {
        //                case "ItemChanged":
        //                    await ContextService.RunAsync(async () =>
        //                    {
        //                        try
        //                        {
        //                            var item = await CustomerService.GetCustomerAsync(current.CustomerID);
        //                            item = item ?? new CustomerModel { CustomerID = current.CustomerID, IsEmpty = true };
        //                            current.Merge(item);
        //                            current.NotifyChanges();
        //                            NotifyPropertyChanged(nameof(Title));
        //                            if (IsEditMode)
        //                            {
        //                                StatusMessage("WARNING: This customer has been modified externally");
        //                            }
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            LogException("Customer", "Handle Changes", ex);
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

        protected override void SendItemChangedMessage(string message, long itemId)
            => Messenger.Send(new CustomerChangedMessage(message, itemId));
    }
}
