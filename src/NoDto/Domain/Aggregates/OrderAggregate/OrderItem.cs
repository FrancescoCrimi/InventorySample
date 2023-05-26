// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.

using Inventory.Domain.Aggregates.ProductAggregate;

namespace Inventory.Domain.Aggregates.OrderAggregate
{
    public class OrderItem : Infrastructure.Common.ObservableObject<OrderItem>
    {
        #region fields

        private int orderLine;
        private int quantity;
        private decimal unitPrice;
        private decimal discount;
        private long orderId;
        private long productId;
        private long taxTypeId;

        #endregion


        #region public property

        public int OrderLine
        {
            get => orderLine;
            set => SetProperty(ref orderLine, value);
        }
        public int Quantity
        {
            get => quantity;
            set => SetProperty(ref quantity, value);
        }
        public decimal UnitPrice
        {
            get => unitPrice;
            set => SetProperty(ref unitPrice, value);
        }
        public decimal Discount
        {
            get => discount;
            set => SetProperty(ref discount, value);
        }

        public long OrderId
        {
            get => orderId;
            set => SetProperty(ref orderId, value);
        }
        public long ProductId
        {
            get => productId;
            set => SetProperty(ref productId, value);
        }
        public long TaxTypeId
        {
            get => taxTypeId;
            set => SetProperty(ref taxTypeId, value);
        }

        #endregion


        #region relation

        public virtual Order Order
        {
            get;
            set;
        }
        public virtual Product Product
        {
            get;
            set;
        }
        public virtual TaxType TaxType
        {
            get;
            set;
        }

        #endregion


        public decimal Subtotal => Quantity * UnitPrice;
        public decimal Total => (Subtotal - Discount) * (1 + TaxType.Rate / 100m);
    }
}
