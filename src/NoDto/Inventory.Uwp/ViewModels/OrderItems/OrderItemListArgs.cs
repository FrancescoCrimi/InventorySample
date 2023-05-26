// Copyright (c) Microsoft. All rights reserved.
// Copyright (c) 2023 Francesco Crimi francrim@gmail.com
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.

using Inventory.Domain.Aggregates.OrderAggregate;
using System;
using System.Linq.Expressions;

namespace Inventory.Uwp.ViewModels.OrderItems
{
    public class OrderItemListArgs
    {
        public OrderItemListArgs()
        {
            OrderBy = r => r.OrderLine;
        }

        public long OrderId { get; set; }

        public string Query { get; set; }

        public Expression<Func<OrderItem, object>> OrderBy { get; set; }
        public Expression<Func<OrderItem, object>> OrderByDesc { get; set; }
    }
}
