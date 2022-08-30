using CiccioSoft.Inventory.Infrastructure.Common;
using CiccioSoft.Inventory.Infrastructure.Logging;
using Inventory.UwpApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.UwpApp.Services
{
    public class LogServiceFacade
    {
        private readonly LogService logService;

        public LogServiceFacade(LogService logService)
        {
            this.logService = logService;
        }

        internal Task<int> GetLogsCountAsync(DataRequest<Log> dataRequest)
        {
            return logService.GetLogsCountAsync(dataRequest);
        }

        internal async Task<IList<LogModel>> GetLogsAsync(int v, int rangeSize, DataRequest<Log> dataRequest)
        {
            var items = await logService.GetLogsAsync(v, rangeSize, dataRequest);
            IList<LogModel> models = new List<LogModel>();
            foreach (var item in items)
            {
                var model = CreateLogModel(item);
                models.Add(model);
            }
            return models;
        }

        internal Task DeleteLogAsync(LogModel model)
        {
            Log item = new Log { Id = (int)model.Id };
            return logService.DeleteLogAsync(item);
        }

        internal Task DeleteLogRangeAsync(int index, int length, DataRequest<Log> request)
        {
            return logService.DeleteLogRangeAsync(index, length, request);
        }

        internal async Task<LogModel> GetLogAsync(long logId)
        {
            var item = await logService.GetLogAsync(logId);
            var model = CreateLogModel(item);
            return model;
        }


        private LogModel CreateLogModel(Log source)
        {
            return new LogModel()
            {
                Id = source.Id,
                //IsRead = source.IsRead,
                DateTime = source.DateTime,
                //User = source.User,
                //Type = source.Type,
                Level = source.Level,
                Source = source.Source,
                Action = source.Action,
                Message = source.Message,
                //Description = source.Description,
            };
        }
    }
}
