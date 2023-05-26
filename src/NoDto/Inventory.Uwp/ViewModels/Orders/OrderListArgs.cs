// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.

using System;
using System.Linq.Expressions;
using Inventory.Domain.Aggregates.OrderAggregate;

namespace Inventory.Uwp.ViewModels.Orders
{
    public class OrderListArgs
    {
        public OrderListArgs()
        {
            OrderByDesc = r => r.OrderDate;
        }

        public long CustomerId { get; set; }

        public string Query { get; set; }

        public bool IsMainView { get; set; }

        public Expression<Func<Order, object>> OrderBy { get; set; }
        public Expression<Func<Order, object>> OrderByDesc { get; set; }
    }
}
