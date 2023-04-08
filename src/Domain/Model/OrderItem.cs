﻿#region copyright
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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.DependencyInjection;
using Inventory.Domain.Common;

namespace Inventory.Domain.Model
{
    [Table("OrderItems")]
    public partial class OrderItem : ObservableObject<OrderItem>
    {
        //[Key]
        //[DatabaseGenerat‌​ed(DatabaseGeneratedOption.None)]
        public long OrderID
        {
            get; set;
        }

        //[Key, Column(Order = 1)]
        //[DatabaseGenerat‌​ed(DatabaseGeneratedOption.None)]
        public int OrderLine
        {
            get; set;
        }

        [Required]
        public long ProductID
        {
            get; set;
        }

        [Required]
        public int Quantity
        {
            get; set;
        }
        [Required]
        public decimal UnitPrice
        {
            get; set;
        }
        [Required]
        public decimal Discount
        {
            get; set;
        }
        [Required]
        public int TaxType
        {
            get; set;
        }

        public virtual Product Product
        {
            get; set;
        }

        public override void Merge(OrderItem source)
        {
            //throw new NotImplementedException();
        }



        public decimal Subtotal => Quantity * UnitPrice;

        public decimal Total => 0;
            //=> (Subtotal - Discount) * (1 + Ioc.Default.GetRequiredService<LookupTableServiceFacade>().GetTaxRate(TaxType) / 100m);

    }
}
