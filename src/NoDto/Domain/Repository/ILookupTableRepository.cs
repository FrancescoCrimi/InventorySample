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
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory.Domain.Repository
{
    public interface ILookupTableRepository : IDisposable
    {
        Task<IList<Category>> GetCategoriesAsync();
        Task<IList<Country>> GetCountryCodesAsync();
        Task<IList<OrderStatus>> GetOrderStatusAsync();
        Task<IList<PaymentType>> GetPaymentTypesAsync();
        Task<IList<Shipper>> GetShippersAsync();
        Task<IList<TaxType>> GetTaxTypesAsync();
    }
}
