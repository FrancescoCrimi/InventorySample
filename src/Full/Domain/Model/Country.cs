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

namespace Inventory.Domain.Model
{
    public partial class Country : IEquatable<Country>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public override bool Equals(object obj) => Equals(obj as Country);
        public bool Equals(Country other) => !(other is null) && Id == other.Id;
        public override int GetHashCode() => HashCode.Combine(Id);
        public static bool operator ==(Country left, Country right)
        {
            return EqualityComparer<Country>.Default.Equals(left, right);
        }
        public static bool operator !=(Country left, Country right)
        {
            return !(left == right);
        }
    }
}
