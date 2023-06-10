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
using Inventory.Infrastructure.DomainBase;

namespace Inventory.Domain.Model
{
    public class OrderItem : Entity<OrderItem>
    {
        #region property

        public int OrderLine
        {
            get; set;
        }
        public int Quantity
        {
            get; set;
        }
        public decimal UnitPrice
        {
            get; set;
        }
        public decimal Discount
        {
            get; set;
        }

        #endregion


        #region relation

        public long OrderId
        {
            get; set;
        }
        public long ProductId
        {
            get; set;
        }
        public long TaxTypeId
        {
            get; set;
        }

        public virtual Order Order
        {
            get; set;
        }
        public virtual Product Product
        {
            get; set;
        }
        public virtual TaxType TaxType
        {
            get; set;
        }

        #endregion

        public decimal Subtotal => Quantity * UnitPrice;
        public decimal Total => (Subtotal - Discount) * (1 + TaxType.Rate / 100m);
    }
}
