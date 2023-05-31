// Copyright (c) 2023 Francesco Crimi francrim@gmail.com
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
using System.Threading.Tasks;

namespace Inventory.Domain.Aggregates.CustomerAggregate
{
    public interface ICustomerRepository : IDisposable
    {
        Task<Customer> GetCustomerAsync(long id);
        Task<IList<Customer>> GetCustomersAsync(int skip, int take, DataRequest<Customer> request);
        Task<IList<Customer>> GetCustomerKeysAsync(int skip, int take, DataRequest<Customer> request);
        Task<int> GetCustomersCountAsync(DataRequest<Customer> request);
        Task<int> UpdateCustomerAsync(Customer customer);
        Task<int> DeleteCustomersAsync(params Customer[] customers);

        Task<List<Country>> GetCountriesAsync();
    }
}
