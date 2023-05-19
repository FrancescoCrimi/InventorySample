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
        private readonly ILogger _logger;
        private readonly LogService _logService;
        private DataRequest<Log> _request;

        public LogCollection(ILogger<LogCollection> logger,
                             LogService logService)
            : base(logger)
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
                //LogException("LogCollection", "Fetch", ex);
                _logger.LogError(LogEvents.Fetch, ex, "Load Log Error");
            }
            return null;
        }
    }
}
