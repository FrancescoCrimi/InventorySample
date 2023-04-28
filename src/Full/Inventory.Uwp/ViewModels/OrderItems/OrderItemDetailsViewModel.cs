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

namespace Inventory.Uwp.ViewModels.OrderItems
{
    public class OrderItemDetailsViewModel : GenericDetailsViewModel<OrderItemDto>
    {
        private readonly ILogger _logger;
        private readonly OrderItemService _orderItemService;

        public OrderItemDetailsViewModel(ILogger<OrderItemDetailsViewModel> logger,
                                         OrderItemService orderItemService)
            : base()
        {
            _logger = logger;
            _orderItemService = orderItemService;
        }

        public override string Title => Item?.IsNew ?? true ? TitleNew : TitleEdit;
        public string TitleNew => $"New Order Item, Order #{OrderID}";
        public string TitleEdit => $"Order Line {Item?.OrderLine}, #{Item?.OrderId}" ?? string.Empty;

        public override bool ItemIsNew => Item?.IsNew ?? true;

        public OrderItemDetailsArgs ViewModelArgs { get; private set; }

        public long OrderID { get; set; }

        public ICommand ProductSelectedCommand => new RelayCommand<ProductDto>(ProductSelected);
        private void ProductSelected(ProductDto product)
        {
            EditableItem.ProductId = product.Id;
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
                Item = new OrderItemDto { OrderId = OrderID };
                IsEditMode = true;
            }
            else
            {
                try
                {
                    var item = await _orderItemService.GetOrderItemAsync(OrderID, ViewModelArgs.OrderLine);
                    Item = item ?? new OrderItemDto { OrderId = OrderID, OrderLine = ViewModelArgs.OrderLine, IsEmpty = true };
                }
                catch (Exception ex)
                {
                    _logger.LogError(LogEvents.Load, ex, "Load OrderItem");
                }
            }
        }
        public void Unload()
        {
            ViewModelArgs.OrderID = Item?.OrderId ?? 0;
        }

        public void Subscribe()
        {
            //MessageService.Subscribe<OrderItemDetailsViewModel, OrderItemModel>(this, OnDetailsMessage);
            Messenger.Register<ItemMessage<OrderItemDto>>(this, OnOrderItemMessage);

            //MessageService.Subscribe<OrderItemListViewModel>(this, OnListMessage);
            Messenger.Register<ItemMessage<IList<OrderItemDto>>>(this, OnOrderItemListMessage);
            Messenger.Register<ItemMessage<IList<IndexRange>>>(this, OnIndexRangeListMessage);
        }

        public void Unsubscribe()
        {
            //MessageService.Unsubscribe(this);
            Messenger.UnregisterAll(this);
        }

        public OrderItemDetailsArgs CreateArgs()
        {
            return new OrderItemDetailsArgs
            {
                OrderID = Item?.OrderId ?? 0,
                OrderLine = Item?.OrderLine ?? 0
            };
        }

        protected override async Task<bool> SaveItemAsync(OrderItemDto model)
        {
            try
            {
                StartStatusMessage("Saving order item...");
                await Task.Delay(100);
                await _orderItemService.UpdateOrderItemAsync(model);
                EndStatusMessage("Order item saved");
                _logger.LogInformation(LogEvents.Save, $"Order item #{model.OrderId}, {model.OrderLine} was saved successfully.");
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Error saving Order item: {ex.Message}");
                _logger.LogError(LogEvents.Save, ex, "Error saving Order item");
                return false;
            }
        }

        protected override async Task<bool> DeleteItemAsync(OrderItemDto model)
        {
            try
            {
                StartStatusMessage("Deleting order item...");
                await Task.Delay(100);
                await _orderItemService.DeleteOrderItemAsync(model);
                EndStatusMessage("Order item deleted");
                _logger.LogWarning(LogEvents.Delete, $"Order item #{model.OrderId}, {model.OrderLine} was deleted.");
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Error deleting Order item: {ex.Message}");
                _logger.LogError(LogEvents.Delete, ex, "Error deleting Order item");
                return false;
            }
        }

        protected override async Task<bool> ConfirmDeleteAsync()
        {
            return await ShowDialogAsync("Confirm Delete", "Are you sure you want to delete current order item?", "Ok", "Cancel");
        }

        protected override IEnumerable<IValidationConstraint<OrderItemDto>> GetValidationConstraints(OrderItemDto model)
        {
            yield return new RequiredConstraint<OrderItemDto>("Product", m => m.ProductId);
            yield return new NonZeroConstraint<OrderItemDto>("Quantity", m => m.Quantity);
            yield return new PositiveConstraint<OrderItemDto>("Quantity", m => m.Quantity);
            yield return new LessThanConstraint<OrderItemDto>("Quantity", m => m.Quantity, 100);
            yield return new PositiveConstraint<OrderItemDto>("Discount", m => m.Discount);
            yield return new NonGreaterThanConstraint<OrderItemDto>("Discount", m => m.Discount, (double)model.Subtotal, "'Subtotal'");
        }

        /*
         *  Handle external messages
         ****************************************************************/

        private async void OnOrderItemMessage(object recipient, ItemMessage<OrderItemDto> message)
        {
            //    throw new NotImplementedException();
            //}
            //private async void OnDetailsMessage(OrderItemDetailsViewModel sender, string message, OrderItemModel changed)
            //{
            var current = Item;
            if (current != null)
            {
                if (message.Value != null && message.Value.OrderId == current?.OrderId && message.Value.OrderLine == current?.OrderLine)
                {
                    switch (message.Message)
                    {
                        case "ItemChanged":
                            //await ContextService.RunAsync(async () =>
                            //{
                            try
                            {
                                var item = await _orderItemService.GetOrderItemAsync(current.OrderId, current.OrderLine);
                                item = item ?? new OrderItemDto { OrderId = OrderID, OrderLine = ViewModelArgs.OrderLine, IsEmpty = true };
                                current.Merge(item);
                                current.NotifyChanges();
                                OnPropertyChanged(nameof(Title));
                                if (IsEditMode)
                                {
                                    StatusMessage("WARNING: This orderItem has been modified externally");
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(LogEvents.HandleChanges, ex, "Handle OrderItem Changes");
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
                            var model = await _orderItemService.GetOrderItemAsync(current.OrderId, current.OrderLine);
                            if (model == null)
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(LogEvents.HandleRangesDeleted, ex, "Handle OrderItem Ranges Deleted");
                        }
                        break;
                }
            }
        }
        private async void OnOrderItemListMessage(object recipient, ItemMessage<IList<OrderItemDto>> message)
        {
            var current = Item;
            if (current != null)
            {
                switch (message.Message)
                {
                    case "ItemsDeleted":
                        if (message.Value.Any(r => r.OrderId == current.OrderId && r.OrderLine == current.OrderLine))
                        {
                            await OnItemDeletedExternally();
                        }
                        break;
                }
            }
        }
        //private async void OnListMessage(OrderItemListViewModel sender, string message, object args)
        //{
        //    var current = Item;
        //    if (current != null)
        //    {
        //        switch (message)
        //        {
        //            case "ItemsDeleted":
        //                if (args is IList<OrderItemModel> deletedModels)
        //                {
        //                    if (deletedModels.Any(r => r.OrderID == current.OrderID && r.OrderLine == current.OrderLine))
        //                    {
        //                        await OnItemDeletedExternally();
        //                    }
        //                }
        //                break;
        //            case "ItemRangesDeleted":
        //                try
        //                {
        //                    var model = await orderItemService.GetOrderItemAsync(current.OrderID, current.OrderLine);
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
                StatusMessage("WARNING: This orderItem has been deleted externally");
            });
            //});
        }
    }
}
