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

using Inventory.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.Infrastructure.Logging
{
    public class LogService
    {
        private readonly IServiceProvider _serviceProvider;

        public LogService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }


        public async Task<Log> GetLogAsync(long id)
        {
            using (var logDbContext = _serviceProvider.GetService<LogDbContext>())
            {
                var item = await logDbContext.Logs.Where(r => r.Id == id).FirstOrDefaultAsync();
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
            using (var logDbContext = _serviceProvider.GetService<LogDbContext>())
            {
                var items = GetLogs(request, logDbContext);
                // Execute
                var records = await items
                    .Skip(skip)
                    .Take(take)
                    .AsNoTracking()
                    .ToListAsync();
                return records;
            }
        }

        public async Task<int> GetLogsCountAsync(DataRequest<Log> request)
        {
            using (var logDbContext = _serviceProvider.GetService<LogDbContext>())
            {
                IQueryable<Log> items = logDbContext.Logs;

                // Query
                if (!string.IsNullOrEmpty(request.Query))
                {
                    items = items.Where(r => r.Message.Contains(request.Query.ToLower()));
                }

                // Where
                if (request.Where != null)
                {
                    items = items.Where(request.Where);
                }

                var ret = await items.CountAsync();
                return ret;
            }
        }

        public async Task<int> DeleteLogAsync(Log log)
        {
            using (var logDbContext = _serviceProvider.GetService<LogDbContext>())
            {
                logDbContext.Logs.RemoveRange(log);
                return await logDbContext.SaveChangesAsync();
            }
        }

        public async Task<int> DeleteLogRangeAsync(int index, int length, DataRequest<Log> request)
        {
            using (var logDbContext = _serviceProvider.GetService<LogDbContext>())
            {
                var items = GetLogs(request, logDbContext);

                // Execute
                var records = await items
                    .Skip(index)
                    .Take(length)
                    .Select(r => new Log
                    {
                        Id = r.Id,
                    })
                    .AsNoTracking()
                    .ToListAsync();

                logDbContext.Logs.RemoveRange(records);
                return await logDbContext.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsReadAsync()
        {
            using (var logDbContext = _serviceProvider.GetService<LogDbContext>())
            {
                var items = await logDbContext.Logs.Where(r => !r.IsRead).ToListAsync();
                foreach (var item in items)
                {
                    item.IsRead = true;
                }
                await logDbContext.SaveChangesAsync();
            }
        }




        public static event EventHandler AddLogEvent;

        public static void RaiseNewEventLog()
        {
            AddLogEvent?.Invoke(null, new EventArgs());
        }


        private IQueryable<Log> GetLogs(DataRequest<Log> request, LogDbContext logDbContext)
        {
            IQueryable<Log> items = logDbContext.Logs;

            // Query
            if (!string.IsNullOrEmpty(request.Query))
            {
                items = items.Where(r => r.Message.Contains(request.Query.ToLower()));
            }

            // Where
            if (request.Where != null)
            {
                items = items.Where(request.Where);
            }

            // Order By
            if (request.OrderBy != null)
            {
                items = items.OrderBy(request.OrderBy);
            }
            if (request.OrderByDesc != null)
            {
                items = items.OrderByDescending(request.OrderByDesc);
            }

            return items;
        }
    }
}
