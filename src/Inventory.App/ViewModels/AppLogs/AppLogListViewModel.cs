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

using CiccioSoft.Inventory.Data;
using CiccioSoft.Inventory.Uwp.Models;
using CiccioSoft.Inventory.Uwp.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CiccioSoft.Inventory.Uwp.ViewModels
{
    #region AppLogListArgs
    public class AppLogListArgs
    {
        static public AppLogListArgs CreateEmpty() => new AppLogListArgs { IsEmpty = true };

        public AppLogListArgs()
        {
            OrderByDesc = r => r.Logged;
        }

        public bool IsEmpty { get; set; }

        public string Query { get; set; }

        public Expression<Func<Log, object>> OrderBy { get; set; }
        public Expression<Func<Log, object>> OrderByDesc { get; set; }
    }
    #endregion

    public class AppLogListViewModel : GenericListViewModel<AppLogModel>
    {
        private readonly ILogger logger;
        private readonly ILogService logService;
        private readonly IDialogService dialogService;

        public AppLogListViewModel(ILogger<AppLogListViewModel> logger,
                                   ILogService logService,
                                   IDialogService dialogService)
            : base()
        {
            this.logger = logger;
            this.logService = logService;
            this.dialogService = dialogService;
        }

        public AppLogListArgs ViewModelArgs { get; private set; }

        public async Task LoadAsync(AppLogListArgs args)
        {
            ViewModelArgs = args ?? AppLogListArgs.CreateEmpty();
            Query = ViewModelArgs.Query;

            StartStatusMessage("Loading logs...");
            if (await RefreshAsync())
            {
                EndStatusMessage("Logs loaded");
            }
        }
        public void Unload()
        {
            ViewModelArgs.Query = Query;
        }

        public void Subscribe()
        {
            //MessageService.Subscribe<AppLogListViewModel>(this, OnMessage);
            //MessageService.Subscribe<AppLogDetailsViewModel>(this, OnMessage);
            Messenger.Register<ItemMessage<AppLogModel>>(this, OnItemMessage);

            // LogService non salva piu i log
            //MessageService.Subscribe<ILogService, Log>(this, OnLogServiceMessage);
        }

        private async void OnItemMessage(object recipient, ItemMessage<AppLogModel> message)
        {
            switch (message.Message)
            {
                //case "NewItemSaved":
                case "ItemDeleted":
                case "ItemsDeleted":
                    //case "ItemRangesDeleted":
                    await RefreshAsync();
                    break;
            }
        }

        //private async void OnMessage(ViewModelBase sender, string message, object args)
        //{
        //    switch (message)
        //    {
        //        case "NewItemSaved":
        //        case "ItemDeleted":
        //        case "ItemsDeleted":
        //        case "ItemRangesDeleted":
        //            //await ContextService.RunAsync(async () =>
        //            //{
        //            await RefreshAsync();
        //            //});
        //            break;
        //    }
        //}



        public void Unsubscribe()
        {
            //MessageService.Unsubscribe(this);
            Messenger.UnregisterAll(this);
        }

        public AppLogListArgs CreateArgs()
        {
            return new AppLogListArgs
            {
                Query = Query,
                OrderBy = ViewModelArgs.OrderBy,
                OrderByDesc = ViewModelArgs.OrderByDesc
            };
        }

        public async Task<bool> RefreshAsync()
        {
            bool isOk = true;

            Items = null;
            ItemsCount = 0;
            SelectedItem = null;

            try
            {
                Items = await GetItemsAsync();
            }
            catch (Exception ex)
            {
                Items = new List<AppLogModel>();
                StatusError($"Error loading Logs: {ex.Message}");
                logger.LogError(ex, "Refresh");
                isOk = false;
            }

            ItemsCount = Items.Count;
            if (!IsMultipleSelection)
            {
                SelectedItem = Items.FirstOrDefault();
            }
            OnPropertyChanged(nameof(Title));

            return isOk;
        }

        private async Task<IList<AppLogModel>> GetItemsAsync()
        {
            if (!ViewModelArgs.IsEmpty)
            {
                DataRequest<Log> request = BuildDataRequest();
                return await logService.GetLogsAsync(request);
            }
            return new List<AppLogModel>();
        }

        protected override void OnNew()
        {
            throw new NotImplementedException();
        }

        protected override async void OnRefresh()
        {
            StartStatusMessage("Loading logs...");
            if (await RefreshAsync())
            {
                EndStatusMessage("Logs loaded");
            }
        }

        protected override async void OnDeleteSelection()
        {
            StatusReady();
            if (await dialogService.ShowAsync("Confirm Delete", "Are you sure you want to delete selected logs?", "Ok", "Cancel"))
            {
                int count = 0;
                try
                {
                    if (SelectedIndexRanges != null)
                    {
                        count = SelectedIndexRanges.Sum(r => r.Length);
                        StartStatusMessage($"Deleting {count} logs...");
                        await DeleteRangesAsync(SelectedIndexRanges);
                        //MessageService.Send(this, "ItemRangesDeleted", SelectedIndexRanges);
                        Messenger.Send(new ItemMessage<IList<IndexRange>>(SelectedIndexRanges, "ItemRangesDeleted"));
                    }
                    else if (SelectedItems != null)
                    {
                        count = SelectedItems.Count();
                        StartStatusMessage($"Deleting {count} logs...");
                        await DeleteItemsAsync(SelectedItems);
                        //MessageService.Send(this, "ItemsDeleted", SelectedItems);
                        Messenger.Send(new ItemMessage<IList<AppLogModel>>(SelectedItems, "ItemsDeleted"));
                    }
                }
                catch (Exception ex)
                {
                    StatusError($"Error deleting {count} Logs: {ex.Message}");
                    logger.LogError(ex, "Delete");
                    count = 0;
                }
                await RefreshAsync();
                SelectedIndexRanges = null;
                SelectedItems = null;
                if (count > 0)
                {
                    EndStatusMessage($"{count} logs deleted");
                }
            }
        }

        private async Task DeleteItemsAsync(IEnumerable<AppLogModel> models)
        {
            foreach (var model in models)
            {
                await logService.DeleteLogAsync(model);
            }
        }

        private async Task DeleteRangesAsync(IEnumerable<IndexRange> ranges)
        {
            DataRequest<Log> request = BuildDataRequest();
            foreach (var range in ranges)
            {
                await logService.DeleteLogRangeAsync(range.Index, range.Length, request);
            }
        }

        private DataRequest<Log> BuildDataRequest()
        {
            return new DataRequest<Log>()
            {
                Query = Query,
                OrderBy = ViewModelArgs.OrderBy,
                OrderByDesc = ViewModelArgs.OrderByDesc
            };
        }

        //private async void OnLogServiceMessage(ILogService logService, string message, Log log)
        //{
        //    if (message == "LogAdded")
        //    {
        //        //await ContextService.RunAsync(async () =>
        //        //{
        //            await RefreshAsync();
        //        //});
        //    }
        //}
    }
}
