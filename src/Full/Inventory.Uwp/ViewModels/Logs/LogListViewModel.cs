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
using Inventory.Uwp.Dto;
using Inventory.Uwp.Library.Common;
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
    public class LogListViewModel : ViewModelBase
    {
        private readonly ILogger _logger;
        private readonly LogService _logService;
        private readonly LogCollection _collection;

        public LogListViewModel(ILogger<LogListViewModel> logger,
                                LogService logService,
                                LogCollection logCollection)
            : base()
        {
            this._logger = logger;
            this._logService = logService;
            _collection = logCollection;
            Items = _collection;
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
                await _collection.LoadAsync(request);
            }
            catch (Exception ex)
            {
                Items = new List<Log>();
                StatusError($"Error loading Logs: {ex.Message}");
                _logger.LogError(LogEvents.Refresh, ex, "Error loading Logs");
                isOk = false;
            }

            ItemsCount = Items.Count;
            OnPropertyChanged(nameof(Title));
            return isOk;
        }

        protected  void OnNew()
        {
            throw new NotImplementedException();
        }

        protected async  void OnRefresh()
        {
            StartStatusMessage("Loading logs...");
            if (await RefreshAsync())
            {
                EndStatusMessage("Logs loaded");
            }
        }

        protected async  void OnDeleteSelection()
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
                        Messenger.Send(new ItemMessage<IList<Log>>(SelectedItems, "ItemsDeleted"));
                    }
                }
                catch (Exception ex)
                {
                    StatusError($"Error deleting {count} Logs: {ex.Message}");
                    _logger.LogError(LogEvents.Delete, ex, "Error deleting {count} Logs");
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

        private async Task DeleteItemsAsync(IEnumerable<Log> models)
        {
            foreach (var model in models)
            {
                await _logService.DeleteLogAsync(model);
            }
        }

        private async Task DeleteRangesAsync(IEnumerable<IndexRange> ranges)
        {
            DataRequest<Log> request = BuildDataRequest();
            foreach (var range in ranges)
            {
                await _logService.DeleteLogRangeAsync(range.Index, range.Length, request);
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














        private IList<Log> _items = null;
        public IList<Log> Items
        {
            get
            {
                return _items;
            }
            set
            {
                SetProperty(ref _items, value);
            }
        }

        private string _query = null;
        public string Query
        {
            get => _query;
            set => SetProperty(ref _query, value);
        }

        private int _itemsCount = 0;
        public int ItemsCount
        {
            get => _itemsCount;
            set => SetProperty(ref _itemsCount, value);
        }

        public List<Log> SelectedItems { get; protected set; }
        public IndexRange[] SelectedIndexRanges { get; protected set; }



        private Log _selectedItem = default;
        public Log SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                if (SetProperty(ref _selectedItem, value))
                {
                    if (!IsMultipleSelection)
                    {
                        // fix _selectedItem == null
                        if (_selectedItem != null)
                        {
                            // Todo: fixare selectedItem.Id = 0
                            ////MessageService.Send(this, "ItemSelected", _selectedItem);
                            var message = new ItemMessage<Log>(_selectedItem, "ItemSelected");
                            Messenger.Send(message);
                        }
                    }
                }
            }
        }

        private bool _isMultipleSelection = false;
        public bool IsMultipleSelection
        {
            get => _isMultipleSelection;
            set => SetProperty(ref _isMultipleSelection, value);
        }
    }
}
