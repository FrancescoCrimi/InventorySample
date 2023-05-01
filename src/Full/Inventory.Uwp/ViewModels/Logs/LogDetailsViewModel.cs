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

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Inventory.Infrastructure.Logging;
using Inventory.Uwp.Dto;
using Inventory.Uwp.Library.Common;
using Inventory.Uwp.Services;
using Inventory.Uwp.ViewModels.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Inventory.Uwp.ViewModels.Logs
{
    public class LogDetailsViewModel : ViewModelBase
    {
        private readonly ILogger _logger;
        private readonly LogService _logService;
        private readonly NavigationService _navigationService;

        public LogDetailsViewModel(ILogger<LogListViewModel> logger,
                                   LogService logService,
                                   NavigationService navigationService)
            : base()
        {
            _logger = logger;
            _logService = logService;
            this._navigationService = navigationService;
        }

        public override string Title => "Activity Logs";

        public bool ItemIsNew => false;

        public LogDetailsArgs ViewModelArgs { get; private set; }

        public async Task LoadAsync(LogDetailsArgs args)
        {
            ViewModelArgs = args ?? LogDetailsArgs.CreateDefault();

            try
            {
                var item = await _logService.GetLogAsync(ViewModelArgs.AppLogID);
                Item = item ?? new Log { Id = 0/*, IsEmpty = true*/ };
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
            Messenger.Register<LogMessage>(this, OnDetailsMessage);
        }

        public void Unsubscribe()
        {
            Messenger.UnregisterAll(this);
        }

        public LogDetailsArgs CreateArgs()
        {
            return new LogDetailsArgs
            {
                AppLogID = Item?.Id ?? 0
            };
        }

        protected Task<bool> SaveItemAsync(Log model)
        {
            throw new NotImplementedException();
        }

        protected async Task<bool> DeleteItemAsync(Log model)
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

        protected async Task<bool> ConfirmDeleteAsync()
        {
            return await ShowDialogAsync("Confirm Delete", "Are you sure you want to delete current log?", "Ok", "Cancel");
        }

        /*
         *  Handle external messages
         ****************************************************************/

        private async void OnDetailsMessage(object recipient, LogMessage message)
        {
            var current = Item;
            if (current != null)
            {
                switch (message.Value)
                {
                    case "ItemDeleted":
                        if (message.Id != 0 && message.Id == current?.Id)
                        {
                            await OnItemDeletedExternally();
                        }
                        break;
                    case "ItemsDeleted":
                        if (message.Items != null)
                        {
                            if (message.Items.Any(r => r.Id == current.Id))
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

        private async Task OnItemDeletedExternally()
        {
            await Task.Run(() =>
            {
                //CancelEdit();
                //IsEnabled = false;
                StatusMessage("WARNING: This log has been deleted externally");
            });
        }



























        private Log _item = null;
        public Log Item
        {
            get => _item;
            set
            {
                if (SetProperty(ref _item, value))
                {
                    //EditableItem = _item;
                    //IsEnabled = !_item?.IsEmpty ?? false;
                    OnPropertyChanged(nameof(IsDataAvailable));
                    OnPropertyChanged(nameof(IsDataUnavailable));
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        public bool IsDataAvailable => _item != null;
        public bool IsDataUnavailable => !IsDataAvailable;




        public ICommand DeleteCommand => new RelayCommand(OnDelete);
        protected virtual async void OnDelete()
        {
            StatusReady();
            if (await ConfirmDeleteAsync())
            {
                await DeleteAsync();
            }
        }
        public virtual async Task DeleteAsync()
        {
            var model = Item;
            if (model != null)
            {
                //IsEnabled = false;
                if (await DeleteItemAsync(model))
                {
                    //MessageService.Send(this, "ItemDeleted", model);
                    Messenger.Send(new LogMessage("ItemDeleted", model.Id));
                }
                //else
                //{
                //    IsEnabled = true;
                //}
            }
        }



        public bool CanGoBack => !IsMainView && _navigationService.CanGoBack;

        public ICommand BackCommand => new RelayCommand(OnBack);
        protected virtual void OnBack()
        {
            StatusReady();
            if (_navigationService.CanGoBack)
            {
                _navigationService.GoBack();
            }
        }

        public bool IsEditMode = false;
        public bool IsEnabled = true;
    }
}
