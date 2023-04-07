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
using Inventory.Uwp.ViewModels.Common;
using Inventory.Uwp.ViewModels.Message;
using Microsoft.Extensions.Logging;

namespace Inventory.Uwp.ViewModels.OrderItems
{
    public class OrderItemDetailsViewModel : GenericDetailsViewModel<OrderItem>
    {
        private readonly ILogger<OrderItemDetailsViewModel> _logger;
        private readonly IOrderItemService _orderItemService;

        public OrderItemDetailsViewModel(ILogger<OrderItemDetailsViewModel> logger,
                                         IOrderItemService orderItemService)
            : base()
        {
            _logger = logger;
            _orderItemService = orderItemService;
        }

        public override string Title => Item?.IsNew ?? true ? TitleNew : TitleEdit;
        public string TitleNew => $"New Order Item, Order #{OrderID}";
        public string TitleEdit => $"Order Line {Item?.OrderLine}, #{Item?.OrderID}" ?? string.Empty;

        public override bool ItemIsNew => Item?.IsNew ?? true;

        public OrderItemDetailsArgs ViewModelArgs
        {
            get; private set;
        }

        public long OrderID
        {
            get; set;
        }

        public ICommand ProductSelectedCommand => new RelayCommand<Product>(ProductSelected);
        private void ProductSelected(Product product)
        {
            EditableItem.ProductID = product.Id;
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
                Item = new OrderItem { OrderID = OrderID };
                IsEditMode = true;
            }
            else
            {
                try
                {
                    var item = await _orderItemService.GetOrderItemAsync(OrderID, ViewModelArgs.OrderLine);
                    Item = item ?? new OrderItem { OrderID = OrderID, OrderLine = ViewModelArgs.OrderLine, IsEmpty = true };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Load");
                }
            }
        }
        public void Unload()
        {
            ViewModelArgs.OrderID = Item?.OrderID ?? 0;
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
                OrderID = Item?.OrderID ?? 0,
                OrderLine = Item?.OrderLine ?? 0
            };
        }

        protected async override Task<bool> SaveItemAsync(OrderItem model)
        {
            try
            {
                StartStatusMessage("Saving order item...");
                await Task.Delay(100);
                await _orderItemService.UpdateOrderItemAsync(model);
                EndStatusMessage("Order item saved");
                _logger.LogInformation($"Order item #{model.OrderID}, {model.OrderLine} was saved successfully.");
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Error saving Order item: {ex.Message}");
                _logger.LogError(ex, "Save");
                return false;
            }
        }

        protected async override Task<bool> DeleteItemAsync(OrderItem model)
        {
            try
            {
                StartStatusMessage("Deleting order item...");
                await Task.Delay(100);
                await _orderItemService.DeleteOrderItemAsync(model);
                EndStatusMessage("Order item deleted");
                _logger.LogWarning($"Order item #{model.OrderID}, {model.OrderLine} was deleted.");
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Error deleting Order item: {ex.Message}");
                _logger.LogError(ex, "Delete");
                return false;
            }
        }

        protected async override Task<bool> ConfirmDeleteAsync()
        {
            return await ShowDialogAsync("Confirm Delete", "Are you sure you want to delete current order item?", "Ok", "Cancel");
        }

        protected override IEnumerable<IValidationConstraint<OrderItem>> GetValidationConstraints(OrderItem model)
        {
            yield return new RequiredConstraint<OrderItem>("Product", m => m.ProductID);
            yield return new NonZeroConstraint<OrderItem>("Quantity", m => m.Quantity);
            yield return new PositiveConstraint<OrderItem>("Quantity", m => m.Quantity);
            yield return new LessThanConstraint<OrderItem>("Quantity", m => m.Quantity, 100);
            yield return new PositiveConstraint<OrderItem>("Discount", m => m.Discount);
            yield return new NonGreaterThanConstraint<OrderItem>("Discount", m => m.Discount, (double)model.Subtotal, "'Subtotal'");
        }

        /*
         *  Handle external messages
         ****************************************************************/

        public void Subscribe()
        {
            //MessageService.Subscribe<OrderItemDetailsViewModel, OrderItemModel>(this, OnDetailsMessage);
            //MessageService.Subscribe<OrderItemListViewModel>(this, OnListMessage);
            Messenger.Register<OrderItemChangeMessage>(this, OnMessage);
        }

        private async void OnMessage(object recipient, OrderItemChangeMessage message)
        {
            var current = Item;
            if (current != null)
            {
                switch (message.Value)
                {
                    case "ItemChanged":
                        if (message.OrderID == current?.OrderID && message.OrderLine == current?.OrderLine)
                        {
                            try
                            {
                                var item = await _orderItemService.GetOrderItemAsync(current.OrderID, current.OrderLine);
                                item = item ?? new OrderItem { OrderID = OrderID, OrderLine = ViewModelArgs.OrderLine, IsEmpty = true };
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
                                _logger.LogError(ex, "Handle Changes");
                            }
                        }
                        break;

                    case "ItemDeleted":
                        if (message.OrderID == current?.OrderID && message.OrderLine == current?.OrderLine)
                        {
                            await OnItemDeletedExternally();
                        }
                        break;


                    case "ItemRangesDeleted":
                        try
                        {
                            var model = await _orderItemService.GetOrderItemAsync(current.OrderID, current.OrderLine);
                            if (model == null)
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Handle Ranges Deleted");
                        }
                        break;

                    case "ItemsDeleted":
                        if (message.SelectedItems.Any(r => r.OrderID == current.OrderID && r.OrderLine == current.OrderLine))
                        {
                            await OnItemDeletedExternally();
                        }
                        break;
                }
            }
        }

        //private async void OnDetailsMessage(OrderItemDetailsViewModel sender, string message, OrderItemModel changed)
        //{
        //    var current = Item;
        //    if (current != null)
        //    {
        //        if (changed != null && changed.OrderID == current?.OrderID && changed.OrderLine == current?.OrderLine)
        //        {
        //            switch (message)
        //            {
        //                case "ItemChanged":
        //                    await ContextService.RunAsync(async () =>
        //                    {
        //                        try
        //                        {
        //                            var item = await OrderItemService.GetOrderItemAsync(current.OrderID, current.OrderLine);
        //                            item = item ?? new OrderItemModel { OrderID = OrderID, OrderLine = ViewModelArgs.OrderLine, IsEmpty = true };
        //                            current.Merge(item);
        //                            current.NotifyChanges();
        //                            NotifyPropertyChanged(nameof(Title));
        //                            if (IsEditMode)
        //                            {
        //                                StatusMessage("WARNING: This orderItem has been modified externally");
        //                            }
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            LogException("OrderItem", "Handle Changes", ex);
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
        //                    var model = await OrderItemService.GetOrderItemAsync(current.OrderID, current.OrderLine);
        //                    if (model == null)
        //                    {
        //                        await OnItemDeletedExternally();
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    LogException("OrderItem", "Handle Ranges Deleted", ex);
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

        protected override void SendItemChangedMessage(string message, long itemId)
            => Messenger.Send(new OrderItemChangeMessage(message, Item.OrderID, Item.OrderLine));
    }
}
