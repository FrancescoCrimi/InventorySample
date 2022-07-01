using CiccioSoft.Inventory.Data;
using CiccioSoft.Inventory.Domain.Repository;
using CiccioSoft.Inventory.Infrastructure.Common;
using CiccioSoft.Inventory.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CiccioSoft.Inventory.Persistence.Repository
{
    internal class LogRepository : ILogRepository
    {
        private readonly LogDbContext logDbContext;

        public LogRepository(LogDbContext logDbContext)
        {
            this.logDbContext = logDbContext;
        }

        //public async Task<int> CreateLogAsync(AppLog appLog)
        //{
        //    appLog.DateTime = DateTime.UtcNow;
        //    appLogDbContext.Entry(appLog).State = EntityState.Added;
        //    return await appLogDbContext.SaveChangesAsync();
        //}

        public async Task<int> DeleteLogsAsync(params Log[] logs)
        {
            logDbContext.Logs.RemoveRange(logs);
            return await logDbContext.SaveChangesAsync();
        }

        public async Task<Log> GetLogAsync(long id)
        {
            return await logDbContext.Logs.Where(r => r.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IList<Log>> GetLogKeysAsync(int skip, int take, DataRequest<Log> request)
        {
            IQueryable<Log> items = GetLogs(request);

            // Execute
            var records = await items.Skip(skip).Take(take)
                .Select(r => new Log
                {
                    Id = r.Id,
                })
                .AsNoTracking()
                .ToListAsync();

            return records;
        }

        public async Task<IList<Log>> GetLogsAsync(int skip, int take/*, DataRequest<Log> request*/)
        {
            IQueryable<Log> items = logDbContext.Logs;

            // Execute
            var records = await items.Skip(skip).Take(take)
                .AsNoTracking()
                .ToListAsync();

            return records;
        }

        public async Task<int> GetLogsCountAsync(DataRequest<Log> request)
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

            return await items.CountAsync();
        }

        public async Task MarkAllAsReadAsync()
        {
            //var items = await appLogDbContext.Logs.Where(r => !r.IsRead).ToListAsync();
            //foreach (var item in items)
            //{
            //    item.IsRead = true;
            //}
            //await appLogDbContext.SaveChangesAsync();
            await Task.CompletedTask;
        }


        private IQueryable<Log> GetLogs(DataRequest<Log> request)
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

        public void Dispose()
        {
            logDbContext.Dispose();
        }
    }
}
