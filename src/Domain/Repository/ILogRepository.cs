using CiccioSoft.Inventory.Domain.Model;
using CiccioSoft.Inventory.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CiccioSoft.Inventory.Domain.Repository
{
    public interface ILogRepository : IDisposable
    {
        Task<int> DeleteLogsAsync(params Log[] logs);
        Task<Log> GetLogAsync(long id);
        Task<IList<Log>> GetLogKeysAsync(int skip, int take, DataRequest<Log> request);
        Task<IList<Log>> GetLogsAsync(int skip, int take/*, DataRequest<Log> request*/);
        Task<int> GetLogsCountAsync(DataRequest<Log> request);
        Task MarkAllAsReadAsync();
    }
}