using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inventory.Infrastructure.Common;
using Inventory.Infrastructure.Logging;
using Inventory.Uwp.Library.Common;
using Microsoft.Extensions.Logging;

namespace Inventory.Uwp.Services.VirtualCollections
{
    public class LogCollection : VirtualRangeCollection<Log>
    {
        private readonly ILogger<LogCollection> _logger;
        private readonly LogService _logService;
        private DataRequest<Log> _request;

        public LogCollection(ILogger<LogCollection> logger,
                             LogService logService)
        {
            _logger = logger;
            _logService = logService;
        }

        public async Task LoadAsync(DataRequest<Log> request)
        {
            _request = request;
            await LoadAsync();
        }
        protected override Log CreateDummyEntity()
            => new Log { Id = -1, Message = "Dummy Log", Description = "Dummy Log" };
        protected override Task<int> GetCountAsync()
            => _logService.GetLogsCountAsync(_request);
        protected async override Task<IList<Log>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken)
        {
            try
            {
                var list = await _logService.GetLogsAsync(skip, take, _request);
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Load logs error");
                throw ex;
            }
        }
    }
}
