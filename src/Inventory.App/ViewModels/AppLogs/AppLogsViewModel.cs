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

using CiccioSoft.Inventory.Data.Models;
using CiccioSoft.Inventory.Uwp.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Threading.Tasks;

namespace CiccioSoft.Inventory.Uwp.ViewModels
{
    public class AppLogsViewModel : ViewModelBase
    {
        private readonly ILogger<AppLogsViewModel> logger;
        private readonly ILogService logService;

        public AppLogsViewModel(ILogger<AppLogsViewModel> logger,
                                ILogService logService,
                                AppLogListViewModel appLogListViewModel,
                                AppLogDetailsViewModel appLogDetailsViewModel)
            : base()
        {
            this.logger = logger;
            this.logService = logService;
            AppLogList = appLogListViewModel;
            AppLogDetails = appLogDetailsViewModel;
        }

        public AppLogListViewModel AppLogList { get; }
        public AppLogDetailsViewModel AppLogDetails { get; }

        public async Task LoadAsync(AppLogListArgs args)
        {
            await AppLogList.LoadAsync(args);
        }
        public void Unload()
        {
            AppLogList.Unload();
        }

        public void Subscribe()
        {
            //MessageService.Subscribe<AppLogListViewModel>(this, OnMessage);
            Messenger.Register<ItemMessage<AppLogModel>>(this, OnMessage);
            AppLogList.Subscribe();
            AppLogDetails.Subscribe();
        }

        public void Unsubscribe()
        {
            //MessageService.Unsubscribe(this);
            Messenger.UnregisterAll(this);
            AppLogList.Unsubscribe();
            AppLogDetails.Unsubscribe();
        }

        private async void OnMessage(object recipient, ItemMessage<AppLogModel> message)
        {
            if (/*recipient == AppLogList &&*/ message.Message == "ItemSelected")
            {
                await OnItemSelected();
            }
        }

        private async Task OnItemSelected()
        {
            if (AppLogDetails.IsEditMode)
            {
                StatusReady();
            }
            var selected = AppLogList.SelectedItem;
            if (!AppLogList.IsMultipleSelection)
            {
                if (selected != null && !selected.IsEmpty)
                {
                    await PopulateDetails(selected);
                }
            }
            AppLogDetails.Item = selected;
        }

        private async Task PopulateDetails(AppLogModel selected)
        {
            try
            {
                var model = await logService.GetLogAsync(selected.Id);
                selected.Merge(model);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Load Details");
            }
        }
    }
}
