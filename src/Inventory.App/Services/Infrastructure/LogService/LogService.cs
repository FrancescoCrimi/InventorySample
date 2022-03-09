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

using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Inventory.Data;
using Inventory.Models;
using Inventory.Data.DataContexts;

namespace Inventory.Services
{
    public class LogService : ILogService
    {
        public LogService(IMessageService messageService)
        {
            MessageService = messageService;
        }

        public IMessageService MessageService { get; }

        //public async Task WriteAsync(LogType type, string source, string action, Exception ex)
        //{
        //    await WriteAsync(LogType.Error, source, action, ex.Message, ex.ToString());
        //    Exception deepException = ex.InnerException;
        //    while (deepException != null)
        //    {
        //        await WriteAsync(LogType.Error, source, action, deepException.Message, deepException.ToString());
        //        deepException = deepException.InnerException;
        //    }
        //}
        //public async Task WriteAsync(LogType type, string source, string action, string message, string description)
        //{
        //    var appLog = new AppLog()
        //    {
        //        User = AppSettings.Current.UserName ?? "App",
        //        Type = type,
        //        Source = source,
        //        Action = action,
        //        Message = message,
        //        Description = description,
        //    };

        //    appLog.IsRead = type != LogType.Error;

        //    await CreateLogAsync(appLog);
        //    MessageService.Send(this, "LogAdded", appLog);
        //}

        private LogDbContext CreateDataSource()
        {
            return new LogDbContext(AppSettings.Current.LogConnectionString);
        }



        public async Task<AppLogModel> GetLogAsync(long id)
        {
            using (var dc = CreateDataSource())
            {
                var repo = new LogRepository(dc);
                var item = await repo.GetLogAsync(id);
                if (item != null)
                {
                    return CreateAppLogModel2(item);
                }
                return null;
            }
        }

        public async Task<IList<AppLogModel>> GetLogsAsync(DataRequest<Log> request)
        {
            var collection = new LogCollection(this);
            await collection.LoadAsync(request);
            return collection;
        }

        public async Task<IList<AppLogModel>> GetLogsAsync(int skip, int take, DataRequest<Log> request)
        {
            var models = new List<AppLogModel>();
            //using (var ds = CreateDataSource())
            //{
            //    var items = await ds.GetLogsAsync(skip, take, request);
            //    foreach (var item in items)
            //    {
            //        models.Add(CreateAppLogModel(item));
            //    }
            //    return models;
            //}
            using (var dc = CreateDataSource())
            {
                var repo = new LogRepository(dc);
                var items = await repo.GetLogsAsync(skip, take/*, request*/);
                foreach (var item in items)
                {
                    models.Add(CreateAppLogModel2(item));
                }
                return models;
            }
        }

        public async Task<int> GetLogsCountAsync(DataRequest<Log> request)
        {
            using (var dc = CreateDataSource())
            {
                var repo = new LogRepository(dc);
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

        public async Task<int> DeleteLogAsync(AppLogModel model)
        {
            var appLog = new Log { Id = (int)model.Id };
            using (var dc = CreateDataSource())
            {
                var repo = new LogRepository(dc);
                return await repo.DeleteLogsAsync(appLog);
            }
        }

        public async Task<int> DeleteLogRangeAsync(int index, int length, DataRequest<Log> request)
        {
            using (var dc = CreateDataSource())
            {
                var repo = new LogRepository(dc);
                var items = await repo.GetLogKeysAsync(index, length, request);
                return await repo.DeleteLogsAsync(items.ToArray());
            }
        }

        public async Task MarkAllAsReadAsync()
        {
            using (var dc = CreateDataSource())
            {
                var repo = new LogRepository(dc);
                await repo.MarkAllAsReadAsync();
            }
        }

        //private AppLogModel CreateAppLogModel(AppLog source)
        //{
        //    return new AppLogModel()
        //    {
        //        Id = source.Id,
        //        IsRead = source.IsRead,
        //        DateTime = source.DateTime,
        //        User = source.User,
        //        Type = source.Type,
        //        Source = source.Source,
        //        Action = source.Action,
        //        Message = source.Message,
        //        Description = source.Description,
        //    };
        //}

        private AppLogModel CreateAppLogModel2(Log source)
        {
            return new AppLogModel()
            {
                Id = source.Id,
                //IsRead = source.IsRead,
                DateTime = source.Logged,
                //User = source.User,
                //Type = source.Type,
                Source = source.Logger,
                Action = source.Callsite,
                Message = source.Message,
                //Description = source.Description,
            };
        }


    }
}
