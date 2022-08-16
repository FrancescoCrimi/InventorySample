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

using CiccioSoft.Inventory.Infrastructure.Common;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CiccioSoft.Inventory.Infrastructure.Logging
{
    public class LogService /*: ILogService*/
    {
        private readonly IServiceProvider serviceProvider;

        public static event EventHandler AddLogEvent;

        public static void RaiseNewEventLog()
        {
            if (AddLogEvent != null)
            {
                AddLogEvent.Invoke(null, new EventArgs());
            }
        }

        public LogService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task<Log> GetLogAsync(long id)
        {
            using (var repo = serviceProvider.GetService<ILogRepository>())
            {
                var item = await repo.GetLogAsync(id);
                return item;
            }
        }

        //public async Task<IList<AppLogModel>> GetLogsAsync(DataRequest<Log> request)
        //{
        //    var collection = new LogCollection(this);
        //    await collection.LoadAsync(request);
        //    return collection;
        //}

        public async Task<IList<Log>> GetLogsAsync(int skip, int take, DataRequest<Log> request)
        {
            using (var repo = serviceProvider.GetService<ILogRepository>())
            {
                var items = await repo.GetLogsAsync(skip, take/*, request*/);
                return items;
            }
        }

        public async Task<int> GetLogsCountAsync(DataRequest<Log> request)
        {
            using (var repo = serviceProvider.GetService<ILogRepository>())
            {
                return await repo.GetLogsCountAsync(request);
            }
        }

        //public async Task<int> CreateLogAsync(AppLog appLog)
        //{
        //    using (var ds = CreateDataSource())
        //    {
        //        return await ds.CreateLogAsync(appLog);
        //    }
        //}

        public async Task<int> DeleteLogAsync(Log log)
        {
            using (var repo = serviceProvider.GetService<ILogRepository>())
            {
                return await repo.DeleteLogsAsync(log);
            }
        }

        public async Task<int> DeleteLogRangeAsync(int index, int length, DataRequest<Log> request)
        {
            using (var repo = serviceProvider.GetService<ILogRepository>())
            {
                var items = await repo.GetLogKeysAsync(index, length, request);
                return await repo.DeleteLogsAsync(items.ToArray());
            }
        }

        public async Task MarkAllAsReadAsync()
        {
            using (var repo = serviceProvider.GetService<ILogRepository>())
            {
                await repo.MarkAllAsReadAsync();
            }
        }
    }
}
