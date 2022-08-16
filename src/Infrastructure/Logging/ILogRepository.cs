using CiccioSoft.Inventory.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CiccioSoft.Inventory.Infrastructure.Logging
{
    public interface ILogRepository : IDisposable
    {
        event EventHandler AddLogEvent;
        Task<Log> GetLogAsync(long id);
        Task<IList<Log>> GetLogsAsync(int skip, int take/*, DataRequest<Log> request*/);
        Task<IList<Log>> GetLogKeysAsync(int skip, int take, DataRequest<Log> request);
        Task<int> GetLogsCountAsync(DataRequest<Log> request);
        Task<int> DeleteLogsAsync(params Log[] logs);
        Task<int> DeleteLogRangeAsync(int index, int length, DataRequest<Log> request);
        Task MarkAllAsReadAsync();
    }
}