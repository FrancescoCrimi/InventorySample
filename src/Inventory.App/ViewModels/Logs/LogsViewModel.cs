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

using CiccioSoft.Inventory.Infrastructure.Logging;
using CiccioSoft.Inventory.Uwp.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Threading.Tasks;

namespace CiccioSoft.Inventory.Uwp.ViewModels
{
    public class LogsViewModel : ViewModelBase
    {
        private readonly ILogger<LogsViewModel> logger;
        private readonly LogService logService;

        public LogsViewModel(ILogger<LogsViewModel> logger,
                             LogService logService,
                             LogListViewModel appLogListViewModel,
                             LogDetailsViewModel appLogDetailsViewModel)
            : base()
        {
            this.logger = logger;
            this.logService = logService;
            AppLogList = appLogListViewModel;
            AppLogDetails = appLogDetailsViewModel;
        }

        public LogListViewModel AppLogList { get; }
        public LogDetailsViewModel AppLogDetails { get; }

        public async Task LoadAsync(LogListArgs args)
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
            Messenger.Register<ItemMessage<LogModel>>(this, OnMessage);
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

        private async void OnMessage(object recipient, ItemMessage<LogModel> message)
        {
            if (/*recipient == AppLogList &&*/ message.Message == "ItemSelected")
            {
                await OnItemSelected();
            }
        }

        private async Task OnItemSelected()
        {
            //if (AppLogDetails.IsEditMode)
            //{
                StatusReady();
            //}
            var selected = AppLogList.SelectedItem;
            if (!AppLogList.IsMultipleSelection)
            {
                if (selected != null /*&& !selected.IsEmpty*/)
                {
                    await PopulateDetails(selected);
                }
            }
            AppLogDetails.Item = selected;
        }

        private async Task<Log> PopulateDetails(LogModel selected)
        {
            try
            {
                var model = await logService.GetLogAsync(selected.Id);
                //selected.Merge(model);
                return model;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Load Details");
                return null;
            }
        }
    }
}
