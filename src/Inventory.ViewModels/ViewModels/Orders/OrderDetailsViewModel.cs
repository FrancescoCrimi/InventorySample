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
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;

namespace Inventory.ViewModels
{
    public class OrderDetailsViewModel : ObservableRecipient //GenericDetailsViewModel<OrderModel>
    {
        private readonly ILogger<OrderDetailsViewModel> logger;
        private readonly IMessageService messageService;
        private readonly IContextService contextService;
        private readonly INavigationService navigationService;
        private readonly IDialogService dialogService;
        private readonly IOrderService orderService;

        public OrderDetailsViewModel(ILogger<OrderDetailsViewModel> logger,
                                     IOrderService orderService,
                                     IMessageService messageService,
                                     IContextService contextService,
                                     INavigationService navigationService,
                                     IDialogService dialogService)
        {
            this.logger = logger;
            this.orderService = orderService;
            this.messageService = messageService;
            this.contextService = contextService;
            this.navigationService = navigationService;
            this.dialogService = dialogService;
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
        virtual public void BeginEdit()
        {
            if (!IsEditMode)
            {
                IsEditMode = true;
                // Create a copy for edit
                var editableItem = new OrderModel();
                editableItem.Merge(Item);
                EditableItem = editableItem;
            }
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
        virtual public Result Validate(OrderModel model)
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






        public string Title => (Item?.IsNew ?? true) ? TitleNew : TitleEdit;
        public string TitleNew => Item?.Customer == null ? "New Order" : $"New Order, {Item?.Customer?.FullName}";
        public string TitleEdit => Item == null ? "Order" : $"Order #{Item?.OrderID}";

        public bool ItemIsNew => Item?.IsNew ?? true;

        public bool CanEditCustomer => Item?.CustomerID <= 0;

        public ICommand CustomerSelectedCommand => new RelayCommand<CustomerModel>(CustomerSelected);
        private void CustomerSelected(CustomerModel customer)
        {
            EditableItem.CustomerID = customer.CustomerID;
            EditableItem.ShipAddress = customer.AddressLine1;
            EditableItem.ShipCity = customer.City;
            EditableItem.ShipRegion = customer.Region;
            EditableItem.ShipCountryCode = customer.CountryCode;
            EditableItem.ShipPostalCode = customer.PostalCode;
            EditableItem.Customer = customer;

            EditableItem.NotifyChanges();
        }

        public OrderDetailsArgs ViewModelArgs { get; private set; }

        public async Task LoadAsync(OrderDetailsArgs args)
        {
            ViewModelArgs = args ?? OrderDetailsArgs.CreateDefault();

            if (ViewModelArgs.IsNew)
            {
                Item = await orderService.CreateNewOrderAsync(ViewModelArgs.CustomerID);
                IsEditMode = true;
            }
            else
            {
                try
                {
                    var item = await orderService.GetOrderAsync(ViewModelArgs.OrderID);
                    Item = item ?? new OrderModel { OrderID = ViewModelArgs.OrderID, IsEmpty = true };
                }
                catch (Exception ex)
                {
                    //LogException("Order", "Load", ex);
                    logger.LogCritical(ex, "Load");
                }
            }
            OnPropertyChanged(nameof(ItemIsNew));
        }
        public void Unload()
        {
            ViewModelArgs.CustomerID = Item?.CustomerID ?? 0;
            ViewModelArgs.OrderID = Item?.OrderID ?? 0;
        }

        public void Subscribe()
        {
            messageService.Subscribe<OrderDetailsViewModel, OrderModel>(this, OnDetailsMessage);
            messageService.Subscribe<OrderListViewModel>(this, OnListMessage);
        }
        public void Unsubscribe()
        {
            messageService.Unsubscribe(this);
        }

        public OrderDetailsArgs CreateArgs()
        {
            return new OrderDetailsArgs
            {
                CustomerID = Item?.CustomerID ?? 0,
                OrderID = Item?.OrderID ?? 0
            };
        }

        protected async Task<bool> SaveItemAsync(OrderModel model)
        {
            //try
            //{
            //StartStatusMessage("Saving order...");
            await Task.Delay(100);
            await orderService.UpdateOrderAsync(model);
            //EndStatusMessage("Order saved");

            //LogInformation("Order", "Save", "Order saved successfully", $"Order #{model.OrderID} was saved successfully.");
            logger.LogInformation($"Order #{model.OrderID} was saved successfully.");

            OnPropertyChanged(nameof(CanEditCustomer));
            return true;
            //}
            //catch (Exception ex)
            //{
            //    StatusError($"Error saving Order: {ex.Message}");
            //    LogException("Order", "Save", ex);
            //    return false;
            //}
        }

        protected async Task<bool> DeleteItemAsync(OrderModel model)
        {
            //try
            //{
            //StartStatusMessage("Deleting order...");
            await Task.Delay(100);
            await orderService.DeleteOrderAsync(model);
            //EndStatusMessage("Order deleted");

            //LogWarning("Order", "Delete", "Order deleted", $"Order #{model.OrderID} was deleted.");
            logger.LogWarning($"Order #{model.OrderID} was deleted.");

            return true;
            //}
            //catch (Exception ex)
            //{
            //    StatusError($"Error deleting Order: {ex.Message}");
            //    LogException("Order", "Delete", ex);
            //    return false;
            //}
        }

        protected async Task<bool> ConfirmDeleteAsync()
        {
            return await dialogService.ShowAsync("Confirm Delete", "Are you sure you want to delete current order?", "Ok", "Cancel");
        }

        protected IEnumerable<IValidationConstraint<OrderModel>> GetValidationConstraints(OrderModel model)
        {
            yield return new RequiredGreaterThanZeroConstraint<OrderModel>("Customer", m => m.CustomerID);
            if (model.Status > 0)
            {
                yield return new RequiredConstraint<OrderModel>("Payment Type", m => m.PaymentType);
                yield return new RequiredGreaterThanZeroConstraint<OrderModel>("Payment Type", m => m.PaymentType);
                if (model.Status > 1)
                {
                    yield return new RequiredConstraint<OrderModel>("Shipper", m => m.ShipVia);
                    yield return new RequiredGreaterThanZeroConstraint<OrderModel>("Shipper", m => m.ShipVia);
                }
            }
        }

        /*
         *  Handle external messages
         ****************************************************************/
        private async void OnDetailsMessage(OrderDetailsViewModel sender, string message, OrderModel changed)
        {
            var current = Item;
            if (current != null)
            {
                if (changed != null && changed.OrderID == current?.OrderID)
                {
                    switch (message)
                    {
                        case "ItemChanged":
                            await contextService.RunAsync(async () =>
                            {
                                try
                                {
                                    var item = await orderService.GetOrderAsync(current.OrderID);
                                    item = item ?? new OrderModel { OrderID = current.OrderID, IsEmpty = true };
                                    current.Merge(item);
                                    current.NotifyChanges();
                                    OnPropertyChanged(nameof(Title));
                                    if (IsEditMode)
                                    {
                                        //StatusMessage("WARNING: This order has been modified externally");
                                        messageService.Send(this, "StatusMessage", "WARNING: This order has been modified externally");

                                    }
                                }
                                catch (Exception ex)
                                {
                                    //LogException("Order", "Handle Changes", ex);
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

        private async void OnListMessage(OrderListViewModel sender, string message, object args)
        {
            var current = Item;
            if (current != null)
            {
                switch (message)
                {
                    case "ItemsDeleted":
                        if (args is IList<OrderModel> deletedModels)
                        {
                            if (deletedModels.Any(r => r.OrderID == current.OrderID))
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        break;
                    case "ItemRangesDeleted":
                        try
                        {
                            var model = await orderService.GetOrderAsync(current.OrderID);
                            if (model == null)
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        catch (Exception ex)
                        {
                            //LogException("Order", "Handle Ranges Deleted", ex);
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
                //StatusMessage("WARNING: This order has been deleted externally");
                messageService.Send(this, "StatusMessage", "WARNING: This order has been deleted externally");
            });
        }





        //virtual public string Title => String.Empty;

        public bool IsDataAvailable => _item != null;
        public bool IsDataUnavailable => !IsDataAvailable;

        public bool CanGoBack => !contextService.IsMainView && navigationService.CanGoBack;

        private OrderModel _item = null;
        public OrderModel Item
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

        private OrderModel _editableItem = null;
        public OrderModel EditableItem
        {
            get => _editableItem;
            set => SetProperty(ref _editableItem, value);
        }

        private bool _isEditMode = false;
        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
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
