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

using CiccioSoft.Inventory.Uwp.Models;
using CiccioSoft.Inventory.Uwp.Services;
using CiccioSoft.Inventory.Uwp.Services.Infrastructure;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CiccioSoft.Inventory.Uwp.ViewModels
{
    public abstract partial class GenericDetailsViewModel<TModel> : ViewModelBase where TModel : ObservableObject, new()
    {
        private readonly NavigationService navigationService;
        private readonly WindowService windowService;

        public GenericDetailsViewModel()
            : base()
        {
            navigationService = Ioc.Default.GetService<NavigationService>();
            windowService = Ioc.Default.GetService<WindowService>();
        }

        public LookupTableServiceFacade LookupTables => Ioc.Default.GetRequiredService<LookupTableServiceFacade>();

        public bool IsDataAvailable => _item != null;
        public bool IsDataUnavailable => !IsDataAvailable;

        public bool CanGoBack => !IsMainView && navigationService.CanGoBack;

        private TModel _item = null;
        public TModel Item
        {
            get
            {
                return _item;
            }

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

        private TModel _editableItem = null;
        public TModel EditableItem
        {
            get => _editableItem;
            set => SetProperty(ref _editableItem, value);
        }

        private bool _isEditMode = false;
        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }

            set
            {
                SetProperty(ref _isEnabled, value);
            }
        }

        public ICommand BackCommand => new RelayCommand(OnBack);
        protected virtual void OnBack()
        {
            StatusReady();
            if (navigationService.CanGoBack)
            {
                navigationService.GoBack();
            }
        }

        public ICommand EditCommand => new RelayCommand(OnEdit);
        protected virtual void OnEdit()
        {
            StatusReady();
            BeginEdit();
            //MessageService.Send(this, "BeginEdit", Item);
            Messenger.Send(new ItemMessage<TModel>(Item, "BeginEdit"));
        }
        public virtual void BeginEdit()
        {
            if (!IsEditMode)
            {
                IsEditMode = true;
                // Create a copy for edit
                var editableItem = new TModel();
                editableItem.Merge(Item);
                EditableItem = editableItem;
            }
        }

        public ICommand CancelCommand => new RelayCommand(OnCancel);
        protected virtual void OnCancel()
        {
            StatusReady();
            CancelEdit();
            //MessageService.Send(this, "CancelEdit", Item);
            Messenger.Send(new ItemMessage<TModel>(Item, "CancelEdit"));
        }
        public virtual void CancelEdit()
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
                    windowService.CloseViewAsync();
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

        public ICommand SaveCommand => new RelayCommand(OnSave);
        protected async virtual void OnSave()
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
        public async virtual Task SaveAsync()
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
                    //MessageService.Send(this, "NewItemSaved", Item);
                    Messenger.Send(new ItemMessage<TModel>(Item, "NewItemSaved"));
                }
                else
                {
                    //MessageService.Send(this, "ItemChanged", Item);
                    Messenger.Send(new ItemMessage<TModel>(Item, "ItemChanged"));
                }
                IsEditMode = false;

                OnPropertyChanged(nameof(ItemIsNew));
            }
            IsEnabled = true;
        }

        public ICommand DeleteCommand => new RelayCommand(OnDelete);
        protected async virtual void OnDelete()
        {
            StatusReady();
            if (await ConfirmDeleteAsync())
            {
                await DeleteAsync();
            }
        }
        public async virtual Task DeleteAsync()
        {
            var model = Item;
            if (model != null)
            {
                IsEnabled = false;
                if (await DeleteItemAsync(model))
                {
                    //MessageService.Send(this, "ItemDeleted", model);
                    Messenger.Send(new ItemMessage<TModel>(model, "ItemDeleted"));
                }
                else
                {
                    IsEnabled = true;
                }
            }
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

        protected virtual IEnumerable<IValidationConstraint<TModel>> GetValidationConstraints(TModel model) => Enumerable.Empty<IValidationConstraint<TModel>>();

        public abstract bool ItemIsNew { get; }

        protected abstract Task<bool> SaveItemAsync(TModel model);
        protected abstract Task<bool> DeleteItemAsync(TModel model);
        protected abstract Task<bool> ConfirmDeleteAsync();
    }
}
