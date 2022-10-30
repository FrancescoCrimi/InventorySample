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

using Inventory.Domain.Model;
using Inventory.Infrastructure.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory.Application
{
    public interface ICustomerService
    {
        Task<Customer> GetCustomerAsync(long id);
        Task<IList<Customer>> GetCustomersAsync(int skip, int take, DataRequest<Customer> request);
        Task<int> GetCustomersCountAsync(DataRequest<Customer> request);
        Task<int> UpdateCustomerAsync(Customer model);
        Task<int> DeleteCustomerAsync(Customer model);
        Task<int> DeleteCustomerRangeAsync(int index, int length, DataRequest<Customer> request);
    }
}
