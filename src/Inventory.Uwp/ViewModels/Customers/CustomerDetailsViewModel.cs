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
using Inventory.Infrastructure.Logging;
using Inventory.Uwp.Common;
using Inventory.Uwp.Dto;
using Inventory.Uwp.Services;
using Inventory.Uwp.ViewModels.Common;
using Inventory.Uwp.ViewModels.Message;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Inventory.Uwp.ViewModels.Customers
{
    public class CustomerDetailsViewModel : GenericDetailsViewModel<CustomerDto>
    {
        private readonly ILogger _logger;
        private readonly CustomerService _customerService;
        private readonly FilePickerService _filePickerService;

        public CustomerDetailsViewModel(ILogger<CustomerDetailsViewModel> logger,
                                        CustomerService customerService,
                                        FilePickerService filePickerService,
                                        NavigationService navigationService,
                                        WindowManagerService windowService,
                                        LookupTablesService lookupTablesService)
            : base(navigationService, windowService, lookupTablesService)
        {
            _logger = logger;
            _customerService = customerService;
            _filePickerService = filePickerService;
        }

        #region public method

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
                    var item = await _customerService.GetCustomerAsync(ViewModelArgs.CustomerId);
                    Item = item ?? new CustomerDto { Id = ViewModelArgs.CustomerId, IsEmpty = true };
                }
                catch (Exception ex)
                {
                    _logger.LogError(LogEvents.Load, ex, "Load Customer");
                }
            }
        }

        public void Unload()
        {
            ViewModelArgs.CustomerId = Item?.Id ?? 0;
        }

        public void Subscribe()
        {
            Messenger.Register<ViewModelsMessage<CustomerDto>>(this, OnMessage);
        }

        public void Unsubscribe()
        {
            Messenger.UnregisterAll(this);
        }

        public CustomerDetailsArgs CreateArgs()
        {
            return new CustomerDetailsArgs
            {
                CustomerId = Item?.Id ?? 0
            };
        }

        public override void BeginEdit()
        {
            NewPictureSource = null;
            base.BeginEdit();
        }

        #endregion


        #region public property

        public override string Title => (Item?.IsNew ?? true) ? "New Customer" : TitleEdit;

        public string TitleEdit => Item == null ? "Customer" : $"{Item.FullName}";

        public override bool ItemIsNew => Item?.IsNew ?? true;

        public CustomerDetailsArgs ViewModelArgs { get; private set; }

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

        #endregion


        #region private and protected method

        protected override async Task<bool> SaveItemAsync(CustomerDto model)
        {
            try
            {
                StartStatusMessage("Saving customer...");
                await Task.Delay(100);
                await _customerService.UpdateCustomerAsync(model);
                EndStatusMessage("Customer saved");
                _logger.LogInformation(LogEvents.Save, $"Customer {model.Id} '{model.FullName}' was saved successfully.");
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Error saving Customer: {ex.Message}");
                _logger.LogError(LogEvents.Save, ex, "Error saving Customer");
                return false;
            }
        }

        protected override async Task<bool> DeleteItemAsync(CustomerDto model)
        {
            try
            {
                StartStatusMessage("Deleting customer...");
                await Task.Delay(100);
                await _customerService.DeleteCustomerAsync(model);
                EndStatusMessage("Customer deleted");
                _logger.LogWarning(LogEvents.Delete, $"Customer {model.Id} '{model.FullName}' was deleted.");
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Error deleting Customer: {ex.Message}");
                _logger.LogError(LogEvents.Delete, ex, "Error deleting Customer");
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
            //yield return new RequiredConstraint<CustomerDto>("Country", m => m.CountryCode);
        }

        private async void OnMessage(object recipient, ViewModelsMessage<CustomerDto> message)
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
                                item = item ?? new CustomerDto { Id = current.Id, IsEmpty = true };
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
                                _logger.LogError(LogEvents.HandleChanges, ex, "Handle Customer Changes");
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
                            _logger.LogError(LogEvents.HandleRangesDeleted, ex, "Handle Customers Ranges Deleted");
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
                StatusMessage("WARNING: This customer has been deleted externally");
            });
        }

        #endregion
    }
}
