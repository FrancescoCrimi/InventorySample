// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Inventory.Infrastructure.Common;
using Inventory.Infrastructure.Logging;
using Inventory.Uwp.Library.Common;
using Inventory.Uwp.Services.VirtualCollections;
using Inventory.Uwp.ViewModels.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Inventory.Uwp.ViewModels.Logs
{
    public class LogListViewModel : ViewModelBase
    {
        private readonly ILogger _logger;
        private readonly LogService _logService;
        private readonly LogCollection _collection;
        private Log _selectedItem = default;
        private IList<Log> _items = null;
        private string _query = null;
        private int _itemsCount = 0;
        private bool _isMultipleSelection = false;

        public LogListViewModel(ILogger<LogListViewModel> logger,
                                LogService logService,
                                LogCollection collection)
            : base()
        {
            _logger = logger;
            _logService = logService;
            _collection = collection;
            Items = collection;
        }

        #region public method

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
            Messenger.Register<LogMessage>(this, OnItemMessage);
        }

        public void Unsubscribe()
        {
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

        #endregion


        #region property

        public LogListArgs ViewModelArgs { get; private set; }

        public Log SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (SetProperty(ref _selectedItem, value))
                {
                    if (!IsMultipleSelection)
                    {
                        // fix _selectedItem == null
                        if (_selectedItem != null)
                        {
                            //MessageService.Send(this, "ItemSelected", _selectedItem);
                            Messenger.Send(new LogMessage("ItemSelected", _selectedItem.Id));
                        }
                    }
                }
            }
        }

        public IList<Log> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        public string Query
        {
            get => _query;
            set => SetProperty(ref _query, value);
        }

        public int ItemsCount
        {
            get => _itemsCount;
            set => SetProperty(ref _itemsCount, value);
        }

        public List<Log> SelectedItems { get; protected set; }

        public IndexRange[] SelectedIndexRanges { get; protected set; }

        public bool IsMultipleSelection
        {
            get => _isMultipleSelection;
            set => SetProperty(ref _isMultipleSelection, value);
        }

        public override string Title => String.IsNullOrEmpty(Query) ? $" ({ItemsCount})" : $" ({ItemsCount} for \"{Query}\")";

        #endregion


        #region icommand property

        public ICommand NewCommand => new RelayCommand(OnNew);
        private void OnNew()
        {
            throw new NotImplementedException();
        }

        public ICommand RefreshCommand => new RelayCommand(OnRefresh);
        private async void OnRefresh()
        {
            StartStatusMessage("Loading logs...");
            if (await RefreshAsync())
            {
                EndStatusMessage("Logs loaded");
            }
        }

        public ICommand StartSelectionCommand => new RelayCommand(OnStartSelection);
        protected virtual void OnStartSelection()
        {
            StatusMessage("Start selection");
            SelectedItem = null;
            SelectedItems = new List<Log>();
            SelectedIndexRanges = null;
            IsMultipleSelection = true;
        }

        public ICommand CancelSelectionCommand => new RelayCommand(OnCancelSelection);
        protected virtual void OnCancelSelection()
        {
            StatusReady();
            SelectedItems = null;
            SelectedIndexRanges = null;
            IsMultipleSelection = false;
            SelectedItem = Items?.FirstOrDefault();
        }

        public ICommand SelectItemsCommand => new RelayCommand<IList<object>>(OnSelectItems);
        protected virtual void OnSelectItems(IList<object> items)
        {
            StatusReady();
            if (IsMultipleSelection)
            {
                SelectedItems.AddRange(items.Cast<Log>());
                StatusMessage($"{SelectedItems.Count} items selected");
            }
        }

        public ICommand DeselectItemsCommand => new RelayCommand<IList<object>>(OnDeselectItems);
        protected virtual void OnDeselectItems(IList<object> items)
        {
            if (items?.Count > 0)
            {
                StatusReady();
            }
            if (IsMultipleSelection)
            {
                foreach (Log item in items)
                {
                    SelectedItems.Remove(item);
                }
                StatusMessage($"{SelectedItems.Count} items selected");
            }
        }

        public ICommand SelectRangesCommand => new RelayCommand<IndexRange[]>(OnSelectRanges);
        protected virtual void OnSelectRanges(IndexRange[] indexRanges)
        {
            SelectedIndexRanges = indexRanges;
            var count = SelectedIndexRanges?.Sum(r => r.Length) ?? 0;
            StatusMessage($"{count} items selected");
        }

        public ICommand DeleteSelectionCommand => new RelayCommand(OnDeleteSelection);
        private async void OnDeleteSelection()
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
                        Messenger.Send(new LogMessage("ItemRangesDeleted", SelectedIndexRanges));
                    }
                    else if (SelectedItems != null)
                    {
                        count = SelectedItems.Count();
                        StartStatusMessage($"Deleting {count} logs...");
                        await DeleteItemsAsync(SelectedItems);
                        //MessageService.Send(this, "ItemsDeleted", SelectedItems);
                        Messenger.Send(new LogMessage("ItemsDeleted", SelectedItems));
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

        #endregion


        #region private method

        private async void OnItemMessage(object recipient, LogMessage message)
        {
            switch (message.Value)
            {
                case "ItemDeleted":
                case "ItemsDeleted":
                case "ItemRangesDeleted":
                    await RefreshAsync();
                    break;
            }
        }

        private async Task<bool> RefreshAsync()
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

        #endregion
    }
}
