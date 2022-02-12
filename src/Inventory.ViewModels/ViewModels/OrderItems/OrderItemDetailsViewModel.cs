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
    public class OrderItemDetailsViewModel : ObservableRecipient //GenericDetailsViewModel<OrderItemModel>
    {
        private readonly IMessageService messageService;
        private readonly IContextService contextService;
        private readonly INavigationService navigationService;
        private readonly IDialogService dialogService;
        private readonly ILogger<OrderItemDetailsViewModel> logger;
        private readonly IOrderItemService orderItemService;

        public OrderItemDetailsViewModel(ILogger<OrderItemDetailsViewModel> logger,
                                         IOrderItemService orderItemService,
                                         IMessageService messageService,
                                         IContextService contextService,
                                         INavigationService navigationService,
                                         IDialogService dialogService)
        {
            this.logger = logger;
            this.orderItemService = orderItemService;
            this.messageService = messageService;
            this.contextService = contextService;
            this.navigationService = navigationService;
            this.dialogService = dialogService;
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
        virtual public void BeginEdit()
        {
            if (!IsEditMode)
            {
                IsEditMode = true;
                // Create a copy for edit
                var editableItem = new OrderItemModel();
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
        virtual public Result Validate(OrderItemModel model)
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







        public string Title => (Item?.IsNew ?? true) ? TitleNew : TitleEdit;
        public string TitleNew => $"New Order Item, Order #{OrderID}";
        public string TitleEdit => $"Order Line {Item?.OrderLine}, #{Item?.OrderID}" ?? String.Empty;

        public bool ItemIsNew => Item?.IsNew ?? true;

        public OrderItemDetailsArgs ViewModelArgs { get; private set; }

        public long OrderID { get; set; }

        public ICommand ProductSelectedCommand => new RelayCommand<ProductModel>(ProductSelected);
        private void ProductSelected(ProductModel product)
        {
            EditableItem.ProductID = product.ProductID;
            EditableItem.UnitPrice = product.ListPrice;
            EditableItem.Product = product;

            EditableItem.NotifyChanges();
        }

        public async Task LoadAsync(OrderItemDetailsArgs args)
        {
            ViewModelArgs = args ?? OrderItemDetailsArgs.CreateDefault();
            OrderID = ViewModelArgs.OrderID;

            if (ViewModelArgs.IsNew)
            {
                Item = new OrderItemModel { OrderID = OrderID };
                IsEditMode = true;
            }
            else
            {
                try
                {
                    var item = await orderItemService.GetOrderItemAsync(OrderID, ViewModelArgs.OrderLine);
                    Item = item ?? new OrderItemModel { OrderID = OrderID, OrderLine = ViewModelArgs.OrderLine, IsEmpty = true };
                }
                catch (Exception ex)
                {
                    //LogException("OrderItem", "Load", ex);
                    logger.LogCritical(ex, "Load");
                }
            }
        }
        public void Unload()
        {
            ViewModelArgs.OrderID = Item?.OrderID ?? 0;
        }

        public void Subscribe()
        {
            messageService.Subscribe<OrderItemDetailsViewModel, OrderItemModel>(this, OnDetailsMessage);
            messageService.Subscribe<OrderItemListViewModel>(this, OnListMessage);
        }
        public void Unsubscribe()
        {
            messageService.Unsubscribe(this);
        }

        public OrderItemDetailsArgs CreateArgs()
        {
            return new OrderItemDetailsArgs
            {
                OrderID = Item?.OrderID ?? 0,
                OrderLine = Item?.OrderLine ?? 0
            };
        }

        protected async Task<bool> SaveItemAsync(OrderItemModel model)
        {
            //try
            //{
            //    StartStatusMessage("Saving order item...");
            await Task.Delay(100);
            await orderItemService.UpdateOrderItemAsync(model);
            //EndStatusMessage("Order item saved");
            
            //LogInformation("OrderItem", "Save", "Order item saved successfully", $"Order item #{model.OrderID}, {model.OrderLine} was saved successfully.");
            logger.LogInformation("Order item saved successfully", $"Order item #{model.OrderID}, {model.OrderLine} was saved successfully.");

            return true;
            //}
            //catch (Exception ex)
            //{
            //    StatusError($"Error saving Order item: {ex.Message}");
            //    LogException("OrderItem", "Save", ex);
            //    return false;
            //}
        }

        protected async Task<bool> DeleteItemAsync(OrderItemModel model)
        {
            //try
            //{
            //    StartStatusMessage("Deleting order item...");
            await Task.Delay(100);
            await orderItemService.DeleteOrderItemAsync(model);
            //EndStatusMessage("Order item deleted");

            //LogWarning("OrderItem", "Delete", "Order item deleted", $"Order item #{model.OrderID}, {model.OrderLine} was deleted.");
            logger.LogWarning("Order item deleted", $"Order item #{model.OrderID}, {model.OrderLine} was deleted.");

            return true;
            //}
            //catch (Exception ex)
            //{
            //    StatusError($"Error deleting Order item: {ex.Message}");
            //    LogException("OrderItem", "Delete", ex);
            //    return false;
            //}
        }

        protected async Task<bool> ConfirmDeleteAsync()
        {
            return await dialogService.ShowAsync("Confirm Delete", "Are you sure you want to delete current order item?", "Ok", "Cancel");
        }

        protected IEnumerable<IValidationConstraint<OrderItemModel>> GetValidationConstraints(OrderItemModel model)
        {
            yield return new RequiredConstraint<OrderItemModel>("Product", m => m.ProductID);
            yield return new NonZeroConstraint<OrderItemModel>("Quantity", m => m.Quantity);
            yield return new PositiveConstraint<OrderItemModel>("Quantity", m => m.Quantity);
            yield return new LessThanConstraint<OrderItemModel>("Quantity", m => m.Quantity, 100);
            yield return new PositiveConstraint<OrderItemModel>("Discount", m => m.Discount);
            yield return new NonGreaterThanConstraint<OrderItemModel>("Discount", m => m.Discount, (double)model.Subtotal, "'Subtotal'");
        }

        /*
         *  Handle external messages
         ****************************************************************/
        private async void OnDetailsMessage(OrderItemDetailsViewModel sender, string message, OrderItemModel changed)
        {
            var current = Item;
            if (current != null)
            {
                if (changed != null && changed.OrderID == current?.OrderID && changed.OrderLine == current?.OrderLine)
                {
                    switch (message)
                    {
                        case "ItemChanged":
                            await contextService.RunAsync(async () =>
                            {
                                try
                                {
                                    var item = await orderItemService.GetOrderItemAsync(current.OrderID, current.OrderLine);
                                    item = item ?? new OrderItemModel { OrderID = OrderID, OrderLine = ViewModelArgs.OrderLine, IsEmpty = true };
                                    current.Merge(item);
                                    current.NotifyChanges();
                                    OnPropertyChanged(nameof(Title));
                                    if (IsEditMode)
                                    {
                                        //StatusMessage("WARNING: This orderItem has been modified externally");
                                        messageService.Send(this, "StatusMessage", "WARNING: This orderItem has been modified externally");

                                    }
                                }
                                catch (Exception ex)
                                {
                                    //LogException("OrderItem", "Handle Changes", ex);
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

        private async void OnListMessage(OrderItemListViewModel sender, string message, object args)
        {
            var current = Item;
            if (current != null)
            {
                switch (message)
                {
                    case "ItemsDeleted":
                        if (args is IList<OrderItemModel> deletedModels)
                        {
                            if (deletedModels.Any(r => r.OrderID == current.OrderID && r.OrderLine == current.OrderLine))
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        break;
                    case "ItemRangesDeleted":
                        try
                        {
                            var model = await orderItemService.GetOrderItemAsync(current.OrderID, current.OrderLine);
                            if (model == null)
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        catch (Exception ex)
                        {
                            //LogException("OrderItem", "Handle Ranges Deleted", ex);
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
                //StatusMessage("WARNING: This orderItem has been deleted externally");
                messageService.Send(this, "StatusMessage", "WARNING: This orderItem has been deleted externally");
            });
        }






        private OrderItemModel _item = null;
        public OrderItemModel Item
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

        private OrderItemModel _editableItem = null;
        public OrderItemModel EditableItem
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
