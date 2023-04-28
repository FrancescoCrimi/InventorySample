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

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Inventory.Infrastructure.Logging;
using Inventory.Uwp.Common;
using Inventory.Uwp.Dto;
using Inventory.Uwp.Library.Common;
using Inventory.Uwp.Services;
using Inventory.Uwp.ViewModels.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Inventory.Uwp.ViewModels.Orders
{
    public class OrderDetailsViewModel : GenericDetailsViewModel<OrderDto>
    {
        private readonly ILogger _logger;
        private readonly OrderService _orderService;

        public OrderDetailsViewModel(ILogger<OrderDetailsViewModel> logger,
                                     OrderService orderService)
            : base()
        {
            _logger = logger;
            _orderService = orderService;
        }

        public override string Title => Item?.IsNew ?? true ? TitleNew : TitleEdit;
        public string TitleNew => Item?.Customer == null ? "New Order" : $"New Order, {Item?.Customer?.FullName}";
        public string TitleEdit => Item == null ? "Order" : $"Order #{Item?.Id}";

        public override bool ItemIsNew => Item?.IsNew ?? true;

        public bool CanEditCustomer => Item?.CustomerId <= 0;

        public ICommand CustomerSelectedCommand => new RelayCommand<CustomerDto>(CustomerSelected);
        private void CustomerSelected(CustomerDto customer)
        {
            EditableItem.CustomerId = customer.Id;
            EditableItem.ShipAddress = customer.AddressLine1;
            EditableItem.ShipCity = customer.City;
            EditableItem.ShipRegion = customer.Region;
            //EditableItem.ShipCountryCode = customer.CountryCode;
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
                Item = await _orderService.CreateNewOrderAsync(ViewModelArgs.CustomerID);
                IsEditMode = true;
            }
            else
            {
                try
                {
                    var item = await _orderService.GetOrderAsync(ViewModelArgs.OrderID);
                    Item = item ?? new OrderDto { Id = ViewModelArgs.OrderID, IsEmpty = true };
                }
                catch (Exception ex)
                {
                    _logger.LogError(LogEvents.Load, ex, "Load Order");
                }
            }
            OnPropertyChanged(nameof(ItemIsNew));
        }
        public void Unload()
        {
            ViewModelArgs.CustomerID = Item?.CustomerId ?? 0;
            ViewModelArgs.OrderID = Item?.Id ?? 0;
        }

        public void Subscribe()
        {
            //MessageService.Subscribe<OrderDetailsViewModel, OrderModel>(this, OnDetailsMessage);
            Messenger.Register<ItemMessage<OrderDto>>(this, OnOrderMessage);

            //MessageService.Subscribe<OrderListViewModel>(this, OnListMessage);
            Messenger.Register<ItemMessage<IList<OrderDto>>>(this, OnOrderListMessage);
            Messenger.Register<ItemMessage<IList<IndexRange>>>(this, OnIndexRangeListMessage);
        }

        public void Unsubscribe()
        {
            //MessageService.Unsubscribe(this);
            Messenger.UnregisterAll(this);
        }

        public OrderDetailsArgs CreateArgs()
        {
            return new OrderDetailsArgs
            {
                CustomerID = Item?.CustomerId ?? 0,
                OrderID = Item?.Id ?? 0
            };
        }

        protected override async Task<bool> SaveItemAsync(OrderDto model)
        {
            try
            {
                StartStatusMessage("Saving order...");
                await Task.Delay(100);
                await _orderService.UpdateOrderAsync(model);
                EndStatusMessage("Order saved");
                _logger.LogInformation(LogEvents.Save, $"Order #{model.Id} was saved successfully.");
                OnPropertyChanged(nameof(CanEditCustomer));
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Error saving Order: {ex.Message}");
                _logger.LogError(LogEvents.Save, ex, "Error saving Order");
                return false;
            }
        }

        protected override async Task<bool> DeleteItemAsync(OrderDto model)
        {
            try
            {
                StartStatusMessage("Deleting order...");
                await Task.Delay(100);
                await _orderService.DeleteOrderAsync(model);
                EndStatusMessage("Order deleted");
                _logger.LogWarning(LogEvents.Delete, $"Order #{model.Id} was deleted.");
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Error deleting Order: {ex.Message}");
                _logger.LogError(LogEvents.Delete, ex, "Error deleting Order");
                return false;
            }
        }

        protected override async Task<bool> ConfirmDeleteAsync()
        {
            return await ShowDialogAsync("Confirm Delete", "Are you sure you want to delete current order?", "Ok", "Cancel");
        }

        protected override IEnumerable<IValidationConstraint<OrderDto>> GetValidationConstraints(OrderDto model)
        {
            yield return new RequiredGreaterThanZeroConstraint<OrderDto>("Customer", m => m.CustomerId);
            if (model.StatusId > 0)
            {
                yield return new RequiredConstraint<OrderDto>("Payment Type", m => m.PaymentTypeId);
                yield return new RequiredGreaterThanZeroConstraint<OrderDto>("Payment Type", m => m.PaymentTypeId);
                if (model.StatusId > 1)
                {
                    yield return new RequiredConstraint<OrderDto>("Shipper", m => m.ShipperId);
                    yield return new RequiredGreaterThanZeroConstraint<OrderDto>("Shipper", m => m.ShipperId);
                }
            }
        }

        /*
         *  Handle external messages
         ****************************************************************/

        private async void OnOrderMessage(object recipient, ItemMessage<OrderDto> message)
        {
            //    throw new NotImplementedException();
            //}
            //private async void OnDetailsMessage(OrderDetailsViewModel sender, string message, OrderModel changed)
            //{
            var current = Item;
            if (current != null)
            {
                if (message.Value != null && message.Value.Id == current?.Id)
                {
                    switch (message.Message)
                    {
                        case "ItemChanged":
                            //await ContextService.RunAsync(async () =>
                            //{
                            try
                            {
                                var item = await _orderService.GetOrderAsync(current.Id);
                                item = item ?? new OrderDto { Id = current.Id, IsEmpty = true };
                                current.Merge(item);
                                current.NotifyChanges();
                                OnPropertyChanged(nameof(Title));
                                if (IsEditMode)
                                {
                                    StatusMessage("WARNING: This order has been modified externally");
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(LogEvents.HandleChanges, ex, "Handle Order Changes");
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
                            var model = await _orderService.GetOrderAsync(current.Id);
                            if (model == null)
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(LogEvents.HandleRangesDeleted, ex, "Handle Order Ranges Deleted");
                        }
                        break;
                }
            }
        }
        private async void OnOrderListMessage(object recipient, ItemMessage<IList<OrderDto>> message)
        {
            var current = Item;
            if (current != null)
            {
                switch (message.Message)
                {
                    case "ItemsDeleted":
                        if (message.Value is IList<OrderDto> deletedModels)
                        {
                            if (deletedModels.Any(r => r.Id == current.Id))
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        break;
                }
            }
        }
        //private async void OnListMessage(OrderListViewModel sender, string message, object args)
        //{
        //    var current = Item;
        //    if (current != null)
        //    {
        //        switch (message)
        //        {
        //            case "ItemsDeleted":
        //                if (args is IList<OrderModel> deletedModels)
        //                {
        //                    if (deletedModels.Any(r => r.OrderID == current.OrderID))
        //                    {
        //                        await OnItemDeletedExternally();
        //                    }
        //                }
        //                break;
        //            case "ItemRangesDeleted":
        //                try
        //                {
        //                    var model = await orderService.GetOrderAsync(current.OrderID);
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
                StatusMessage("WARNING: This order has been deleted externally");
            });
            //});
        }
    }
}
