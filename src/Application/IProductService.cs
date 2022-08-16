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

using CiccioSoft.Inventory.Domain.Model;
using CiccioSoft.Inventory.Infrastructure.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CiccioSoft.Inventory.Application
{
    public interface IProductService
    {
        Task<Product> GetProductAsync(string id);
        Task<IList<Product>> GetProductsAsync(int skip, int take, DataRequest<Product> request);
        Task<int> GetProductsCountAsync(DataRequest<Product> request);
        Task<int> UpdateProductAsync(Product model);
        Task<int> DeleteProductAsync(Product model);
        Task<int> DeleteProductRangeAsync(int index, int length, DataRequest<Product> request);
    }
}
