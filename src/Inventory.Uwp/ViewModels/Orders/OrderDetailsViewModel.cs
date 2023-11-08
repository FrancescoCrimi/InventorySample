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
using Inventory.Uwp.Services;
using Inventory.Uwp.ViewModels.Common;
using Inventory.Uwp.ViewModels.Message;
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
                                     OrderService orderService,
                                     NavigationService navigationService,
                                     WindowManagerService windowService,
                                     LookupTablesService lookupTablesService)
            : base(navigationService, windowService, lookupTablesService)
        {
            _logger = logger;
            _orderService = orderService;
        }

        #region public method

        public async Task LoadAsync(OrderDetailsArgs args)
        {
            ViewModelArgs = args ?? OrderDetailsArgs.CreateDefault();

            if (ViewModelArgs.IsNew)
            {
                //Item = await _orderService.CreateNewOrderAsync(ViewModelArgs.CustomerId);
                Item = new OrderDto();
                IsEditMode = true;
            }
            else
            {
                try
                {
                    var item = await _orderService.GetOrderAsync(ViewModelArgs.OrderId);
                    Item = item ?? new OrderDto { Id = ViewModelArgs.OrderId, IsEmpty = true };
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
            ViewModelArgs.CustomerId = Item?.CustomerId ?? 0;
            ViewModelArgs.OrderId = Item?.Id ?? 0;
        }

        public void Subscribe()
        {
            Messenger.Register<ViewModelsMessage<OrderDto>>(this, OnMessage);
        }

        public void Unsubscribe()
        {
            Messenger.UnregisterAll(this);
        }

        public OrderDetailsArgs CreateArgs()
        {
            return new OrderDetailsArgs
            {
                CustomerId = Item?.CustomerId ?? 0,
                OrderId = Item?.Id ?? 0
            };
        }

        #endregion


        #region public property

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

            //EditableItem.NotifyChanges();
        }

        public OrderDetailsArgs ViewModelArgs { get; private set; }

        #endregion


        #region protected and private method

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
            return await _windowService.OpenDialog("Confirm Delete", "Are you sure you want to delete current order?", "Ok", "Cancel");
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

        private async void OnMessage(object recipient, ViewModelsMessage<OrderDto> message)
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

        private async Task OnItemDeletedExternally()
        {
            await Task.Run(() =>
            {
                CancelEdit();
                IsEnabled = false;
                StatusMessage("WARNING: This order has been deleted externally");
            });
        }

        #endregion
    }
}
