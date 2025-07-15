// Copyright (c) 2023 Francesco Crimi francrim@gmail.com
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.

using Inventory.Domain.Model;
using Inventory.Infrastructure.Common;
using Inventory.Interface.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory.Interface
{
    public interface IOrderServiceFacade
    {
        Task<OrderDto> CreateNewOrderAsync(long customerID);
        Task<int> DeleteOrderAsync(OrderDto model);
        Task<int> DeleteOrderRangeAsync(int index, int length, DataRequest<Order> request);
        Task<OrderDto> GetOrderAsync(long id);
        Task<List<OrderDto>> GetOrdersAsync(int skip, int take, DataRequest<Order> request);
        Task<int> GetOrdersCountAsync(DataRequest<Order> request);
        Task<int> UpdateOrderAsync(OrderDto model);
    }
}