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
using Inventory.Domain.Model;
using Inventory.Domain.Repository;
using Inventory.Infrastructure.Logging;
using Inventory.Uwp.Common;
using Inventory.Uwp.ViewModels.Common;
using Inventory.Uwp.ViewModels.Message;
using Microsoft.Extensions.Logging;

namespace Inventory.Uwp.ViewModels.Orders
{
    public class OrderDetailsViewModel : GenericDetailsViewModel<Order>
    {
        private readonly ILogger _logger;
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerRepository _customerRepository;

        public OrderDetailsViewModel(ILogger<OrderDetailsViewModel> logger,
                                     IOrderRepository orderRepository,
                                     ICustomerRepository customerRepository)
            : base()
        {
            _logger = logger;
            _orderRepository = orderRepository;
            this._customerRepository = customerRepository;
        }

        public override string Title => Item?.IsNew ?? true ? TitleNew : TitleEdit;
        public string TitleNew => Item?.Customer == null ? "New Order" : $"New Order, {Item?.Customer?.FullName}";
        public string TitleEdit => Item == null ? "Order" : $"Order #{Item?.Id}";

        public override bool ItemIsNew => Item?.IsNew ?? true;

        public bool CanEditCustomer => Item?.CustomerId <= 0;

        public ICommand CustomerSelectedCommand => new RelayCommand<Customer>(CustomerSelected);
        private void CustomerSelected(Customer customer)
        {
            EditableItem.CustomerId = customer.Id;
            EditableItem.ShipAddress = customer.AddressLine1;
            EditableItem.ShipCity = customer.City;
            EditableItem.ShipRegion = customer.Region;
            EditableItem.ShipCountryId = customer.CountryId;
            EditableItem.ShipPostalCode = customer.PostalCode;
            EditableItem.Customer = customer;

            EditableItem.NotifyChanges();
        }

        public OrderDetailsArgs ViewModelArgs
        {
            get; private set;
        }

        public async Task LoadAsync(OrderDetailsArgs args)
        {
            ViewModelArgs = args ?? OrderDetailsArgs.CreateDefault();

            if (ViewModelArgs.IsNew)
            {
                //Item = await _orderService.CreateNewOrderAsync(ViewModelArgs.CustomerID);
                var customer = await _customerRepository.GetCustomerAsync(ViewModelArgs.CustomerID);
                Item = Order.CreateNewOrder(customer);
                IsEditMode = true;
            }
            else
            {
                try
                {
                    var item = await _orderRepository.GetOrderAsync(ViewModelArgs.OrderID);
                    Item = item ?? new Order { Id = ViewModelArgs.OrderID, IsEmpty = true };
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
            //MessageService.Subscribe<OrderListViewModel>(this, OnListMessage);
            Messenger.Register<ViewModelsMessage<Order>>(this, OnMessage);
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

        protected async override Task<bool> SaveItemAsync(Order model)
        {
            try
            {
                StartStatusMessage("Saving order...");
                await Task.Delay(100);
                await _orderRepository.UpdateOrderAsync(model);
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

        protected async override Task<bool> DeleteItemAsync(Order model)
        {
            try
            {
                StartStatusMessage("Deleting order...");
                await Task.Delay(100);
                await _orderRepository.DeleteOrdersAsync(model);
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

        protected async override Task<bool> ConfirmDeleteAsync()
        {
            return await ShowDialogAsync("Confirm Delete", "Are you sure you want to delete current order?", "Ok", "Cancel");
        }

        protected override IEnumerable<IValidationConstraint<Order>> GetValidationConstraints(Order model)
        {
            yield return new RequiredGreaterThanZeroConstraint<Order>("Customer", m => m.CustomerId);
            if (model.StatusId > 0)
            {
                yield return new RequiredConstraint<Order>("Payment Type", m => m.PaymentTypeId);
                yield return new RequiredGreaterThanZeroConstraint<Order>("Payment Type", m => m.PaymentTypeId);
                if (model.StatusId > 1)
                {
                    yield return new RequiredConstraint<Order>("Shipper", m => m.ShipperId);
                    yield return new RequiredGreaterThanZeroConstraint<Order>("Shipper", m => m.ShipperId);
                }
            }
        }

        /*
         *  Handle external messages
         ****************************************************************/


        private async void OnMessage(object recipient, ViewModelsMessage<Order> message)
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
                                var item = await _orderRepository.GetOrderAsync(current.Id);
                                item = item ?? new Order { Id = current.Id, IsEmpty = true };
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
                        }
                        break;

                    case "ItemDeleted":
                        if (message.Id != 0 && message.Id == current?.Id)
                        {
                            await OnItemDeletedExternally();
                        }
                        break;

                    case "ItemRangesDeleted":
                        try
                        {
                            var model = await _orderRepository.GetOrderAsync(current.Id);
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

                    case "ItemsDeleted":
                        if (message.SelectedItems != null)
                        {
                            if (message.SelectedItems.Any(r => r.Id == current.Id))
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        break;
                }
            }
        }

        //private async void OnDetailsMessage(OrderDetailsViewModel sender, string message, OrderModel changed)
        //{
        //    var current = Item;
        //    if (current != null)
        //    {
        //        if (changed != null && changed.OrderID == current?.OrderID)
        //        {
        //            switch (message)
        //            {
        //                case "ItemChanged":
        //                    await ContextService.RunAsync(async () =>
        //                    {
        //                        try
        //                        {
        //                            var item = await OrderService.GetOrderAsync(current.OrderID);
        //                            item = item ?? new OrderModel { OrderID = current.OrderID, IsEmpty = true };
        //                            current.Merge(item);
        //                            current.NotifyChanges();
        //                            NotifyPropertyChanged(nameof(Title));
        //                            if (IsEditMode)
        //                            {
        //                                StatusMessage("WARNING: This order has been modified externally");
        //                            }
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            LogException("Order", "Handle Changes", ex);
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
        //                    var model = await OrderService.GetOrderAsync(current.OrderID);
        //                    if (model == null)
        //                    {
        //                        await OnItemDeletedExternally();
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    LogException("Order", "Handle Ranges Deleted", ex);
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
