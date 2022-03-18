using CiccioSoft.Inventory.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CiccioSoft.Inventory.Services
{
    public interface ILogRepository
    {
        //Task<int> CreateLogAsync(Log appLog);
        Task<int> DeleteLogsAsync(params Log[] logs);
        Task<Log> GetLogAsync(long id);
        Task<IList<Log>> GetLogKeysAsync(int skip, int take, DataRequest<Log> request);
        Task<IList<Log>> GetLogsAsync(int skip, int take/*, DataRequest<Log> request*/);
        Task<int> GetLogsCountAsync(DataRequest<Log> request);
        Task MarkAllAsReadAsync();
    }
}