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

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Inventory.Infrastructure.Common;
using Inventory.Uwp.Common;
using Inventory.Uwp.Services;
using Inventory.Uwp.ViewModels.Message;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Inventory.Uwp.ViewModels.Common
{
    public abstract class GenericDetailsViewModel<TModel>
        : ViewModelBase where TModel : Inventory.Infrastructure.Common.ObservableObject<TModel>, new()
    {
        private readonly NavigationService _navigationService;
        private readonly WindowManagerService _windowService;
        private bool _isEditMode = false;
        private bool _isEnabled = true;
        private TModel _item = null;
        private TModel _editableItem = null;
        private RelayCommand _backCommand;
        private RelayCommand _editCommand;
        private RelayCommand _cancelCommand;
        private AsyncRelayCommand _saveCommand;
        private AsyncRelayCommand _deleteCommand;

        public GenericDetailsViewModel(NavigationService navigationService,
                                       WindowManagerService windowService
            //, LookupTablesService lookupTablesService
            )
            : base()
        {
            _navigationService = navigationService;
            _windowService = windowService;
            //LookupTables = lookupTablesService;
        }

        #region public property

        //public LookupTablesService LookupTables { get; }

        public bool IsDataAvailable => _item != null;

        public bool IsDataUnavailable => !IsDataAvailable;

        public bool CanGoBack => !IsMainView && _navigationService.CanGoBack;

        public abstract bool ItemIsNew { get; }

        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        public TModel Item
        {
            get => _item;
            set
            {
                //if (SetProperty(ref _item, value))
                //{
                OnPropertyChanging(nameof(Item));
                _item = value;

                EditableItem = _item;
                IsEnabled = !_item?.IsEmpty ?? false;
                OnPropertyChanged(nameof(Item));
                OnPropertyChanged(nameof(IsDataAvailable));
                OnPropertyChanged(nameof(IsDataUnavailable));
                OnPropertyChanged(nameof(Title));
                //}
            }
        }

        public TModel EditableItem
        {
            get => _editableItem;
            //set => SetProperty(ref _editableItem, value);
            set
            {
                OnPropertyChanging(nameof(EditableItem));
                _editableItem = value;
                OnPropertyChanged(nameof(EditableItem));
            }
        }

        #endregion


        #region command

        public ICommand BackCommand => _backCommand
            ?? (_backCommand = new RelayCommand(OnBack));
        protected virtual void OnBack()
        {
            StatusReady();
            if (_navigationService.CanGoBack)
            {
                _navigationService.GoBack();
            }
        }

        public ICommand EditCommand => _editCommand
            ?? (_editCommand = new RelayCommand(OnEdit));
        protected virtual void OnEdit()
        {
            StatusReady();
            BeginEdit();
            //MessageService.Send(this, "BeginEdit", Item);
            Messenger.Send(new ViewModelsMessage<TModel>("BeginEdit", Item.Id));
        }

        public ICommand CancelCommand => _cancelCommand
            ?? (_cancelCommand = new RelayCommand(OnCancel));
        protected virtual void OnCancel()
        {
            StatusReady();
            CancelEdit();
            Messenger.Send(new ViewModelsMessage<TModel>("CancelEdit", Item.Id));
        }

        public ICommand SaveCommand => _saveCommand
            ?? (_saveCommand = new AsyncRelayCommand(OnSave));
        protected virtual async Task OnSave()
        {
            StatusReady();
            var result = Validate(EditableItem);
            if (result.IsOk)
            {
                await SaveAsync();
            }
            else
            {
                await ShowDialogAsync(result.Message, $"{result.Description} Please, correct the error and try again.");
            }
        }

        public ICommand DeleteCommand => _deleteCommand
            ?? (_deleteCommand = new AsyncRelayCommand(OnDelete));
        protected virtual async Task OnDelete()
        {
            StatusReady();
            if (await ConfirmDeleteAsync())
            {
                await DeleteAsync();
            }
        }

        #endregion


        #region public method

        public virtual async Task SaveAsync()
        {
            IsEnabled = false;
            var isNew = ItemIsNew;
            if (await SaveItemAsync(EditableItem))
            {
                //Item.Merge(EditableItem);
                //Item.NotifyChanges();
                Item = await GetItemAsync(EditableItem.Id);
                OnPropertyChanged(nameof(Title));
                EditableItem = Item;

                if (isNew)
                {
                    //MessageService.Send(this, "NewItemSaved", Item);
                    Messenger.Send(new ViewModelsMessage<TModel>("NewItemSaved", Item.Id));
                }
                else
                {
                    //MessageService.Send(this, "ItemChanged", Item);
                    Messenger.Send(new ViewModelsMessage<TModel>("ItemChanged", Item.Id));
                }
                IsEditMode = false;

                OnPropertyChanged(nameof(ItemIsNew));
            }
            IsEnabled = true;
        }

        public virtual void BeginEdit()
        {
            if (!IsEditMode)
            {
                IsEditMode = true;
                //// Create a copy for edit
                //var editableItem = new TModel();
                //editableItem.Merge(Item);
                //EditableItem = editableItem;
            }
        }

        public virtual void CancelEdit()
        {
            if (ItemIsNew)
            {
                // We were creating a new item: cancel means exit
                if (_navigationService.CanGoBack)
                {
                    _navigationService.GoBack();
                }
                else
                {
                    Task.Run(async () =>
                    {
                        await _windowService.CloseViewAsync();
                    });
                }
                return;
            }

            //// We were editing an existing item: just cancel edition
            if (IsEditMode)
            {
                //EditableItem = Item;
                var item = GetItemAsync(Item.Id).Result;
                EditableItem = item;
            }
            IsEditMode = false;
        }

        public virtual Result Validate(TModel model)
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

        public virtual async Task DeleteAsync()
        {
            var model = Item;
            if (model != null)
            {
                IsEnabled = false;
                if (await DeleteItemAsync(model))
                {
                    // TODO: fix send entity id
                    //MessageService.Send(this, "ItemDeleted", model);
                    Messenger.Send(new ViewModelsMessage<TModel>("ItemDeleted", model.Id));
                }
                else
                {
                    IsEnabled = true;
                }
            }
        }

        #endregion


        #region protected and private method 

        protected virtual IEnumerable<IValidationConstraint<TModel>> GetValidationConstraints(TModel model)
            => Enumerable.Empty<IValidationConstraint<TModel>>();

        protected abstract Task<TModel> GetItemAsync(long id);

        protected abstract Task<bool> SaveItemAsync(TModel model);

        protected abstract Task<bool> DeleteItemAsync(TModel model);

        protected abstract Task<bool> ConfirmDeleteAsync();

        #endregion
    }
}
