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
using Inventory.Infrastructure.Logging;
using Inventory.Uwp.Library.Common;
using Inventory.Uwp.Services;
using Inventory.Uwp.ViewModels.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.Uwp.ViewModels.Logs
{
    #region AppLogDetailsArgs
    public class LogDetailsArgs
    {
        public static LogDetailsArgs CreateDefault() => new LogDetailsArgs();

        public long AppLogID
        {
            get; set;
        }
    }
    #endregion

    public class LogDetailsViewModel : GenericDetailsViewModel<Log>
    {
        private readonly ILogger _logger;
        private readonly LogService _logService;

        public LogDetailsViewModel(ILogger<LogListViewModel> logger,
                                   LogService logService)
            : base()
        {
            _logger = logger;
            _logService = logService;
        }

        public override string Title => "Activity Logs";

        public override bool ItemIsNew => false;

        public LogDetailsArgs ViewModelArgs
        {
            get; private set;
        }

        public async Task LoadAsync(LogDetailsArgs args)
        {
            ViewModelArgs = args ?? LogDetailsArgs.CreateDefault();

            try
            {
                var item = await _logService.GetLogAsync(ViewModelArgs.AppLogID);
                Item = item ?? new Log { Id = 0, IsEmpty = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(LogEvents.Load, ex, "Load");
            }
        }

        public void Unload()
        {
            ViewModelArgs.AppLogID = Item?.Id ?? 0;
        }

        public void Subscribe()
        {
            //MessageService.Subscribe<AppLogDetailsViewModel, AppLogModel>(this, OnDetailsMessage);
            //MessageService.Subscribe<AppLogListViewModel>(this, OnListMessage);
            Messenger.Register<LogMessage>(this, OnMessage);
        }

        public void Unsubscribe()
        {
            //MessageService.Unsubscribe(this);
            Messenger.UnregisterAll(this);
        }

        public LogDetailsArgs CreateArgs()
        {
            return new LogDetailsArgs
            {
                AppLogID = Item?.Id ?? 0
            };
        }

        protected override Task<bool> SaveItemAsync(Log log)
        {
            throw new NotImplementedException();
        }

        protected async override Task<bool> DeleteItemAsync(Log model)
        {
            try
            {
                StartStatusMessage("Deleting log...");
                await Task.Delay(100);
                await _logService.DeleteLogAsync(model);
                EndStatusMessage("Log deleted");
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Error deleting log: {ex.Message}");
                _logger.LogError(LogEvents.Delete, ex, "Error deleting log");
                return false;
            }
        }

        protected async override Task<bool> ConfirmDeleteAsync()
        {
            return await ShowDialogAsync("Confirm Delete", "Are you sure you want to delete current log?", "Ok", "Cancel");
        }

        /*
         *  Handle external messages
         ****************************************************************/

        private async void OnMessage(object recipient, LogMessage message)
        {
            var current = Item;
            if (current != null)
            {
                switch (message.Value)
                {
                    case "ItemDeleted":
                        if (message.Log != null && message.Log.Id == current?.Id)
                        {
                            await OnItemDeletedExternally();
                        }
                        break;
                    case "ItemsDeleted":
                        if (message.SelectedItems is IList<Log> deletedModels)
                        {
                            if (deletedModels.Any(r => r.Id == current.Id))
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        break;
                    case "ItemRangesDeleted":
                        var model = await _logService.GetLogAsync(current.Id);
                        if (model == null)
                        {
                            await OnItemDeletedExternally();
                        }
                        break;
                }
            }
        }

        //private async void OnDetailsMessage(AppLogDetailsViewModel sender, string message, AppLogModel changed)
        //{
        //    var current = Item;
        //    if (current != null)
        //    {
        //        if (changed != null && changed.Id == current?.Id)
        //        {
        //            switch (message)
        //            {
        //                case "ItemDeleted":
        //                    await OnItemDeletedExternally();
        //                    break;
        //            }
        //        }
        //    }
        //}


        //private async void OnListMessage(AppLogListViewModel sender, string message, object args)
        //{
        //    var current = Item;
        //    if (current != null)
        //    {
        //        switch (message)
        //        {
        //            case "ItemsDeleted":
        //                if (args is IList<AppLogModel> deletedModels)
        //                {
        //                    if (deletedModels.Any(r => r.Id == current.Id))
        //                    {
        //                        await OnItemDeletedExternally();
        //                    }
        //                }
        //                break;
        //            case "ItemRangesDeleted":
        //                var model = await LogService.GetLogAsync(current.Id);
        //                if (model == null)
        //                {
        //                    await OnItemDeletedExternally();
        //                }
        //                break;
        //        }
        //    }
        //}


        private async Task OnItemDeletedExternally()
        {
            //await ContextService.RunAsync(() =>
            //{
            await Task.Run(() =>
            {
                //CancelEdit();
                //IsEnabled = false;
                StatusMessage("WARNING: This log has been deleted externally");
            });
            //});
        }

        protected override void SendItemChangedMessage(string message, long itemId) => throw new NotImplementedException();
    }
}
