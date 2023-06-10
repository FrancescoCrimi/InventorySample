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

using CommunityToolkit.Mvvm.Messaging;
using Inventory.Infrastructure.Logging;
using Inventory.Uwp.ViewModels.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Inventory.Uwp.ViewModels.Logs
{
    public class LogsViewModel : ViewModelBase
    {
        private readonly ILogger _logger;
        private readonly LogService _logService;

        public LogsViewModel(ILogger<LogsViewModel> logger,
                             LogService logService,
                             LogListViewModel appLogListViewModel,
                             LogDetailsViewModel appLogDetailsViewModel)
            : base()
        {
            _logger = logger;
            _logService = logService;
            LogList = appLogListViewModel;
            LogDetails = appLogDetailsViewModel;
        }

        public LogListViewModel LogList { get; }

        public LogDetailsViewModel LogDetails { get; }

        public async Task LoadAsync(LogListArgs args)
        {
            await _logService.MarkAllAsReadAsync();
            await LogList.LoadAsync(args);
            if (args != null)
            {
                IsMainView = args.IsMainView;
                LogList.IsMainView = args.IsMainView;
                LogDetails.IsMainView = args.IsMainView;
            }
        }

        public void Unload()
        {
            LogList.Unload();
        }

        public void Subscribe()
        {
            Messenger.Register<LogMessage>(this, OnMessage);
            LogList.Subscribe();
            LogDetails.Subscribe();
        }

        public void Unsubscribe()
        {
            Messenger.UnregisterAll(this);
            LogList.Unsubscribe();
            LogDetails.Unsubscribe();
        }

        private async void OnMessage(object recipient, LogMessage message)
        {
            if (message.Value == "ItemSelected")
            {
                if (message.Id != 0)
                {
                    await OnItemSelected(message.Id);
                }
            }
        }

        private async Task OnItemSelected(long id)
        {
            if (!LogList.IsMultipleSelection)
            {
                if (id != 0)
                {
                    await PopulateDetails(id);
                }
            }
        }

        private async Task PopulateDetails(long id)
        {
            try
            {
                await LogDetails.LoadAsync(new LogDetailsArgs { LogId = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(LogEvents.LoadDetails, ex, "Load Log Details");
            }
        }
    }
}
