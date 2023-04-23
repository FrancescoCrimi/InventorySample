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

using Inventory.Infrastructure.Common;
using Inventory.Infrastructure.Logging;
using Inventory.Uwp.Dto;
using Inventory.Uwp.Library.Common;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inventory.Uwp.Services.VirtualCollections
{
    public class LogCollection : VirtualRangeCollection<LogModel>
    {
        private readonly ILogger<LogCollection> _logger;
        private LogServiceFacade _logService;
        private DataRequest<Log> _request;

        public LogCollection(ILogger<LogCollection> logger,
                             LogServiceFacade logService)
        {
            _logger = logger;
            _logService = logService;
        }

        protected override LogModel CreateDummyEntity()
        {
            return new LogModel();
        }

        protected override Task<int> GetCountAsync()
        {
            return _logService.GetLogsCountAsync(_request);
        }

        protected async override Task<IList<LogModel>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken)
        {
            try
            {
                var list = await _logService.GetLogsAsync(skip, take, _request);
                return list;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Load Log Error");
                throw ex;
            }
        }

        public async Task LoadAsync(DataRequest<Log> request)
        {
            _request = request;
            await LoadAsync();
        }
    }
}
