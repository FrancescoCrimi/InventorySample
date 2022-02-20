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

using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Inventory.Models;
using Inventory.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.Input;

namespace Inventory.ViewModels
{
    public class AppLogDetailsViewModel : ObservableRecipient //GenericDetailsViewModel<AppLogModel>
    {
        private readonly ILogger<AppLogDetailsViewModel> logger;
        private readonly IMessageService messageService;
        private readonly INavigationService navigationService;
        private readonly IContextService contextService;
        private readonly IDialogService dialogService;

        public AppLogDetailsViewModel(ILogger<AppLogDetailsViewModel> logger,
                                      IMessageService messageService,
                                      INavigationService navigationService,
                                      IContextService contextService,
                                      IDialogService dialogService)
        {
            this.logger = logger;
            this.messageService = messageService;
            this.navigationService = navigationService;
            this.contextService = contextService;
            this.dialogService = dialogService;
        }



        public bool CanGoBack => !contextService.IsMainView && navigationService.CanGoBack;

        public ICommand BackCommand => new RelayCommand(OnBack);
        virtual protected void OnBack()
        {
            //StatusReady();
            messageService.Send(this, "StatusMessage", "Ready");
            if (navigationService.CanGoBack)
            {
                navigationService.GoBack();
            }
        }

        public ICommand EditCommand => new RelayCommand(OnEdit);
        virtual protected void OnEdit()
        {
            //StatusReady();
            messageService.Send(this, "StatusMessage", "Ready");
            BeginEdit();
            messageService.Send(this, "BeginEdit", Item);
        }

        virtual public void BeginEdit()
        {
            if (!IsEditMode)
            {
                IsEditMode = true;
                // Create a copy for edit
                var editableItem = new AppLogModel();
                editableItem.Merge(Item);
                EditableItem = editableItem;
            }
        }

        public ICommand DeleteCommand => new RelayCommand(OnDelete);
        virtual protected async void OnDelete()
        {
            //StatusReady();
            messageService.Send(this, "StatusMessage", "Ready");
            if (await ConfirmDeleteAsync())
            {
                await DeleteAsync();
            }
        }
        virtual public async Task DeleteAsync()
        {
            var model = Item;
            if (model != null)
            {
                IsEnabled = false;
                if (await DeleteItemAsync(model))
                {
                    messageService.Send(this, "ItemDeleted", model);
                }
                else
                {
                    IsEnabled = true;
                }
            }
        }

        public ICommand SaveCommand => new RelayCommand(OnSave);
        virtual protected async void OnSave()
        {
            //StatusReady();
            messageService.Send(this, "StatusMessage", "Ready");
            var result = Validate(EditableItem);
            if (result.IsOk)
            {
                await SaveAsync();
            }
            else
            {
                await dialogService.ShowAsync(result.Message, $"{result.Description} Please, correct the error and try again.");
            }
        }
        virtual public async Task SaveAsync()
        {
            IsEnabled = false;
            bool isNew = ItemIsNew;
            if (await SaveItemAsync(EditableItem))
            {
                Item.Merge(EditableItem);
                Item.NotifyChanges();
                OnPropertyChanged(nameof(Title));
                EditableItem = Item;

                if (isNew)
                {
                    messageService.Send(this, "NewItemSaved", Item);
                }
                else
                {
                    messageService.Send(this, "ItemChanged", Item);
                }
                IsEditMode = false;

                OnPropertyChanged(nameof(ItemIsNew));
            }
            IsEnabled = true;
        }

        virtual public Result Validate(AppLogModel model)
        {
            foreach (var constraint in GetValidationConstraints(model))
            {
                if (!constraint.Validate(model))
                {
                    return Result.Error("Validation Error", constraint.Message);
                }
            }
            return Result.Ok();
        }

        virtual protected IEnumerable<IValidationConstraint<AppLogModel>> GetValidationConstraints(AppLogModel model) => Enumerable.Empty<IValidationConstraint<AppLogModel>>();

        public ICommand CancelCommand => new RelayCommand(OnCancel);
        virtual protected void OnCancel()
        {
            //StatusReady();
            messageService.Send(this, "StatusMessage", "Ready");
            CancelEdit();
            messageService.Send(this, "CancelEdit", Item);
        }






        public string Title => "Activity Logs";

        public  bool ItemIsNew => false;

        public AppLogDetailsArgs ViewModelArgs { get; private set; }

        public async Task LoadAsync(AppLogDetailsArgs args)
        {
            ViewModelArgs = args ?? AppLogDetailsArgs.CreateDefault();

            try
            {
                //TODO: LogService
                //var item = await LogService.GetLogAsync(ViewModelArgs.AppLogID);
                //Item = item ?? new AppLogModel { Id = 0, IsEmpty = true };
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                //LogException("AppLog", "Load", ex);
                logger.LogCritical(ex, "Load");
            }
        }

        public void Unload()
        {
            ViewModelArgs.AppLogID = Item?.Id ?? 0;
        }

        public void Subscribe()
        {
            messageService.Subscribe<AppLogDetailsViewModel, AppLogModel>(this, OnDetailsMessage);
            messageService.Subscribe<AppLogListViewModel>(this, OnListMessage);
        }
        public void Unsubscribe()
        {
            messageService.Unsubscribe(this);
        }

        public AppLogDetailsArgs CreateArgs()
        {
            return new AppLogDetailsArgs
            {
                AppLogID = Item?.Id ?? 0
            };
        }

        protected  Task<bool> SaveItemAsync(AppLogModel model)
        {
            throw new NotImplementedException();
        }

        protected  async Task<bool> DeleteItemAsync(AppLogModel model)
        {
            //try
            //{
            //    StartStatusMessage("Deleting log...");
                await Task.Delay(100);

            //TODO: LogService
            //await LogService.DeleteLogAsync(model);

                //EndStatusMessage("Log deleted");
                return true;
            //}
            //catch (Exception ex)
            //{
            //    StatusError($"Error deleting log: {ex.Message}");
            //    LogException("AppLog", "Delete", ex);
            //    return false;
            //}
        }

        protected  async Task<bool> ConfirmDeleteAsync()
        {
            return await dialogService.ShowAsync("Confirm Delete", "Are you sure you want to delete current log?", "Ok", "Cancel");
        }

        /*
         *  Handle external messages
         ****************************************************************/
        private async void OnDetailsMessage(AppLogDetailsViewModel sender, string message, AppLogModel changed)
        {
            var current = Item;
            if (current != null)
            {
                if (changed != null && changed.Id == current?.Id)
                {
                    switch (message)
                    {
                        case "ItemDeleted":
                            await OnItemDeletedExternally();
                            break;
                    }
                }
            }
        }

        private async void OnListMessage(AppLogListViewModel sender, string message, object args)
        {
            var current = Item;
            if (current != null)
            {
                switch (message)
                {
                    case "ItemsDeleted":
                        if (args is IList<AppLogModel> deletedModels)
                        {
                            if (deletedModels.Any(r => r.Id == current.Id))
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        break;
                    case "ItemRangesDeleted":
                        //TODO: LogService
                        //var model = await LogService.GetLogAsync(current.Id);
                        //if (model == null)
                        //{
                        //    await OnItemDeletedExternally();
                        //}
                        break;
                }
            }
        }

        private async Task OnItemDeletedExternally()
        {
            await contextService.RunAsync(() =>
            {
                CancelEdit();
                IsEnabled = false;
                //StatusMessage("WARNING: This log has been deleted externally");
            });
        }






        private AppLogModel _item = null;
        public AppLogModel Item
        {
            get => _item;
            set
            {
                if (SetProperty(ref _item, value))
                {
                    EditableItem = _item;
                    IsEnabled = (!_item?.IsEmpty) ?? false;
                    OnPropertyChanged(nameof(IsDataAvailable));
                    OnPropertyChanged(nameof(IsDataUnavailable));
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        public bool IsDataAvailable => _item != null;
        public bool IsDataUnavailable => !IsDataAvailable;

        private AppLogModel _editableItem = null;
        public AppLogModel EditableItem
        {
            get => _editableItem;
            set => SetProperty(ref _editableItem, value);
        }

        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        virtual public void CancelEdit()
        {
            if (ItemIsNew)
            {
                // We were creating a new item: cancel means exit
                if (navigationService.CanGoBack)
                {
                    navigationService.GoBack();
                }
                else
                {
                    navigationService.CloseViewAsync();
                }
                return;
            }

            // We were editing an existing item: just cancel edition
            if (IsEditMode)
            {
                EditableItem = Item;
            }
            IsEditMode = false;
        }

        private bool _isEditMode = false;
        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }




    }
}
