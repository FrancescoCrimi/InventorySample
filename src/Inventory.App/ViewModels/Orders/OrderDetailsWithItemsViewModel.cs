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

using CiccioSoft.Inventory.Uwp.Models;
using CommunityToolkit.Mvvm.Messaging;
using System.Threading.Tasks;

namespace CiccioSoft.Inventory.Uwp.ViewModels
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

            long orderID = args?.OrderID ?? 0;
            if (orderID > 0)
            {
                await OrderItemList.LoadAsync(new OrderItemListArgs { OrderID = args.OrderID });
            }
            else
            {
                await OrderItemList.LoadAsync(new OrderItemListArgs { IsEmpty = true }, silent: true);
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
            //MessageService.Subscribe<OrderDetailsViewModel, OrderModel>(this, OnMessage);
            Messenger.Register<ItemMessage<OrderModel>>(this, OnOrderMessage);
            OrderDetails.Subscribe();
            OrderItemList.Subscribe();
        }

        public void Unsubscribe()
        {
            //MessageService.Unsubscribe(this);
            Messenger.UnregisterAll(this);
            OrderDetails.Unsubscribe();
            OrderItemList.Unsubscribe();
        }


        private async void OnOrderMessage(object recipient, ItemMessage<OrderModel> message)
        {
        //    throw new NotImplementedException();
        //}
        //private async void OnMessage(OrderDetailsViewModel viewModel, string message, OrderModel order)
        //{
            if (recipient == OrderDetails && message.Message == "ItemChanged")
            {
                await OrderItemList.LoadAsync(new OrderItemListArgs { OrderID = message.Value.OrderID });
            }
        }
    }
}
