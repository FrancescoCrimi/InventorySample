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

using CommunityToolkit.Mvvm.Messaging;
using Inventory.Infrastructure.Common;
using Inventory.Infrastructure.Logging;
using Inventory.Uwp.Library.Common;
using Inventory.Uwp.Models;
using Inventory.Uwp.Services;
using Inventory.Uwp.Services.VirtualCollections;
using Inventory.Uwp.ViewModels.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.Uwp.ViewModels.Logs
{
    public class LogListViewModel : GenericListViewModel<LogModel>
    {
        private readonly ILogger logger;
        private readonly LogServiceFacade logService;
        private readonly LogCollection collection;

        public LogListViewModel(ILogger<LogListViewModel> logger,
                                LogServiceFacade logService)
            : base()
        {
            this.logger = logger;
            this.logService = logService;
            collection = new LogCollection(logService);
            Items = collection;
        }

        public LogListArgs ViewModelArgs { get; private set; }

        public async Task LoadAsync(LogListArgs args)
        {
            ViewModelArgs = args ?? new LogListArgs();
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
            Messenger.Register<ItemMessage<Log>>(this, OnItemMessage);

            // LogService non salva piu i log
            //MessageService.Subscribe<ILogService, Log>(this, OnLogServiceMessage);
        }

        private async void OnItemMessage(object recipient, ItemMessage<Log> message)
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

        public LogListArgs CreateArgs()
        {
            return new LogListArgs
            {
                Query = Query,
                OrderBy = ViewModelArgs.OrderBy,
                OrderByDesc = ViewModelArgs.OrderByDesc
            };
        }

        public async Task<bool> RefreshAsync()
        {
            bool isOk = true;
            ItemsCount = 0;

            try
            {
                DataRequest<Log> request = BuildDataRequest();
                await collection.LoadAsync(request);
            }
            catch (Exception ex)
            {
                Items = new List<LogModel>();
                StatusError($"Error loading Logs: {ex.Message}");
                logger.LogError(ex, "Refresh");
                isOk = false;
            }

            ItemsCount = Items.Count;
            OnPropertyChanged(nameof(Title));
            return isOk;
        }

        protected override void OnNew()
        {
            throw new NotImplementedException();
        }

        protected async override void OnRefresh()
        {
            StartStatusMessage("Loading logs...");
            if (await RefreshAsync())
            {
                EndStatusMessage("Logs loaded");
            }
        }

        protected async override void OnDeleteSelection()
        {
            StatusReady();
            if (await ShowDialogAsync("Confirm Delete", "Are you sure you want to delete selected logs?", "Ok", "Cancel"))
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
                        Messenger.Send(new ItemMessage<IList<LogModel>>(SelectedItems, "ItemsDeleted"));
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

        private async Task DeleteItemsAsync(IEnumerable<LogModel> models)
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
