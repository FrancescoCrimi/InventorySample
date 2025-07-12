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

using Inventory.Infrastructure.Settings;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Inventory.Infrastructure 
{
    public interface IDatabaseMaintenanceService
    {
        Task CopyDatabase(string connectionString,
                          DatabaseProviderType dataProviderType,
                          Action<double> setValue,
                          Action<string> setStatus,
                          CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(string connectionString,
                               DatabaseProviderType dataProviderType,
                               CancellationToken cancellationToken = default);
        string GetDbVersion(string connectionString,
                            DatabaseProviderType dataProviderType);



        //Task<bool> ExistsAsync(DatabaseConfiguration config,
        //                       CancellationToken ct = default);
        //string GetDbVersion(DatabaseConfiguration config);
        //Task CopyDatabase(DatabaseConfiguration source,
        //                  DatabaseConfiguration destination,
        //                  CancellationToken ct = default);
    }
}
