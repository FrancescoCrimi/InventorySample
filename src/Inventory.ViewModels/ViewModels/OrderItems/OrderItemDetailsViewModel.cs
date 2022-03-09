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
using Microsoft.Extensions.Logging;

namespace Inventory.ViewModels
{
    #region OrderItemDetailsArgs
    public class OrderItemDetailsArgs
    {
        static public OrderItemDetailsArgs CreateDefault() => new OrderItemDetailsArgs();

        public long OrderID { get; set; }
        public int OrderLine { get; set; }

        public bool IsNew => OrderLine <= 0;
    }
    #endregion

    public class OrderItemDetailsViewModel : GenericDetailsViewModel<OrderItemModel>
    {
        private readonly ILogger<OrderItemDetailsViewModel> logger;
        private readonly IDialogService dialogService;
        private readonly IOrderItemService orderItemService;

        public OrderItemDetailsViewModel(ILogger<OrderItemDetailsViewModel> logger,
                                         IOrderItemService orderItemService,
                                         IDialogService dialogService)
            : base()
        {
            this.logger = logger;
            this.orderItemService = orderItemService;
            this.dialogService = dialogService;
        }

        override public string Title => (Item?.IsNew ?? true) ? TitleNew : TitleEdit;
        public string TitleNew => $"New Order Item, Order #{OrderID}";
        public string TitleEdit => $"Order Line {Item?.OrderLine}, #{Item?.OrderID}" ?? String.Empty;

        public override bool ItemIsNew => Item?.IsNew ?? true;

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
                    logger.LogError(ex, "Load");
                }
            }
        }
        public void Unload()
        {
            ViewModelArgs.OrderID = Item?.OrderID ?? 0;
        }

        public void Subscribe()
        {
            MessageService.Subscribe<OrderItemDetailsViewModel, OrderItemModel>(this, OnDetailsMessage);
            MessageService.Subscribe<OrderItemListViewModel>(this, OnListMessage);
        }
        public void Unsubscribe()
        {
            MessageService.Unsubscribe(this);
        }

        public OrderItemDetailsArgs CreateArgs()
        {
            return new OrderItemDetailsArgs
            {
                OrderID = Item?.OrderID ?? 0,
                OrderLine = Item?.OrderLine ?? 0
            };
        }

        protected override async Task<bool> SaveItemAsync(OrderItemModel model)
        {
            try
            {
                StartStatusMessage("Saving order item...");
                await Task.Delay(100);
                await orderItemService.UpdateOrderItemAsync(model);
                EndStatusMessage("Order item saved");
                logger.LogInformation($"Order item #{model.OrderID}, {model.OrderLine} was saved successfully.");
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Error saving Order item: {ex.Message}");
                logger.LogError(ex, "Save");
                return false;
            }
        }

        protected override async Task<bool> DeleteItemAsync(OrderItemModel model)
        {
            try
            {
                StartStatusMessage("Deleting order item...");
                await Task.Delay(100);
                await orderItemService.DeleteOrderItemAsync(model);
                EndStatusMessage("Order item deleted");
                logger.LogWarning($"Order item #{model.OrderID}, {model.OrderLine} was deleted.");
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Error deleting Order item: {ex.Message}");
                logger.LogError(ex, "Delete");
                return false;
            }
        }

        protected override async Task<bool> ConfirmDeleteAsync()
        {
            return await dialogService.ShowAsync("Confirm Delete", "Are you sure you want to delete current order item?", "Ok", "Cancel");
        }

        override protected IEnumerable<IValidationConstraint<OrderItemModel>> GetValidationConstraints(OrderItemModel model)
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
                            await ContextService.RunAsync(async () =>
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
                                        StatusMessage("WARNING: This orderItem has been modified externally");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    logger.LogError(ex, "Handle Changes");
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
                            logger.LogError(ex, "Handle Ranges Deleted");
                        }
                        break;
                }
            }
        }

        private async Task OnItemDeletedExternally()
        {
            await ContextService.RunAsync(() =>
            {
                CancelEdit();
                IsEnabled = false;
                StatusMessage("WARNING: This orderItem has been deleted externally");
            });
        }
    }
}
