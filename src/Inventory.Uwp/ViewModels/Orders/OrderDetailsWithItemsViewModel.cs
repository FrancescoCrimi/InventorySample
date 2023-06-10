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

using CommunityToolkit.Mvvm.Messaging;
using Inventory.Uwp.Dto;
using Inventory.Uwp.ViewModels.Common;
using Inventory.Uwp.ViewModels.Message;
using Inventory.Uwp.ViewModels.OrderItems;
using System.Threading.Tasks;

namespace Inventory.Uwp.ViewModels.Orders
{
    public class OrderDetailsWithItemsViewModel : ViewModelBase
    {
        public OrderDetailsWithItemsViewModel(OrderDetailsViewModel orderDetailsViewModel,
                                              OrderItemListViewModel orderItemListViewModel)
            : base()
        {
            OrderDetails = orderDetailsViewModel;
            OrderItemList = orderItemListViewModel;
        }

        public OrderDetailsViewModel OrderDetails { get; set; }
        public OrderItemListViewModel OrderItemList { get; set; }

        public async Task LoadAsync(OrderDetailsArgs args)
        {
            await OrderDetails.LoadAsync(args);

            long orderID = args?.OrderId ?? 0;
            if (orderID > 0)
            {
                await OrderItemList.LoadAsync(new OrderItemListArgs { OrderID = args.OrderId });
            }
            else
            {
                await OrderItemList.LoadAsync(new OrderItemListArgs(), silent: true);
            }
        }
        public void Unload()
        {
            OrderDetails.CancelEdit();
            OrderDetails.Unload();
            OrderItemList.Unload();
        }

        public void Subscribe()
        {
            Messenger.Register<ViewModelsMessage<OrderDto>>(this, OnMessage);
            OrderDetails.Subscribe();
            OrderItemList.Subscribe();
        }

        public void Unsubscribe()
        {
            Messenger.UnregisterAll(this);
            OrderDetails.Unsubscribe();
            OrderItemList.Unsubscribe();
        }


        private async void OnMessage(object recipient, ViewModelsMessage<OrderDto> message)
        {
            if (recipient == OrderDetails && message.Value == "ItemChanged")
            {
                await OrderItemList.LoadAsync(new OrderItemListArgs { OrderID = message.Id });
            }
        }
    }
}
