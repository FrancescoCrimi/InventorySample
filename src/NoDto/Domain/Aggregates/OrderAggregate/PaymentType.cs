// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.

using Inventory.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Domain.Aggregates.OrderAggregate
{
    [Table("PaymentTypes")]
    public class PaymentType : Entity, IEquatable<PaymentType>
    {
        private string name;

        [Required]
        [MaxLength(50)]
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        #region Equals

        public override bool Equals(object obj)
        {
            return Equals(obj as PaymentType);
        }

        public bool Equals(PaymentType other)
        {
            return !(other is null) &&
                   Id == other.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public static bool operator ==(PaymentType left, PaymentType right)
        {
            return EqualityComparer<PaymentType>.Default.Equals(left, right);
        }

        public static bool operator !=(PaymentType left, PaymentType right)
        {
            return !(left == right);
        }

        #endregion
    }
}
