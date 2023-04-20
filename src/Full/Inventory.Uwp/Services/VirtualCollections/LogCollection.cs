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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inventory.Uwp.Services.VirtualCollections
{
    public class LogCollection : VirtualRangeCollection<LogModel>
    {
        private LogServiceFacade logService;
        private DataRequest<Log> request;

        public LogCollection(LogServiceFacade logService)
        {
            this.logService = logService;
        }

        protected override LogModel CreateDummyEntity()
        {
            return new LogModel();
        }

        protected override Task<int> GetCountAsync()
        {
            return logService.GetLogsCountAsync(request);
        }

        protected override Task<IList<LogModel>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken)
        {
            return logService.GetLogsAsync(skip, take, request);
        }

        public async Task LoadAsync(DataRequest<Log> request)
        {
            this.request = request;
            await LoadAsync();
        }
    }
}
