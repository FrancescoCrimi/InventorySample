using CiccioSoft.Inventory.Uwp.Services.Infrastructure;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CiccioSoft.Inventory.Uwp.ViewModels.Common
{
    public abstract class GenericDetailsViewModelReadOnly<TModel> : ViewModelBase where TModel : class, new()
    {
        private readonly NavigationService navigationService;
        private readonly WindowService windowService;

        public GenericDetailsViewModelReadOnly()
        {
            navigationService = Ioc.Default.GetService<NavigationService>();
            windowService = Ioc.Default.GetService<WindowService>();
        }

        private TModel _item = null;
        public TModel Item
        {
            get => _item;
            set
            {
                if (SetProperty(ref _item, value))
                {
                    //EditableItem = _item;
                    //IsEnabled = (!_item?.IsEmpty) ?? false;
                    //OnPropertyChanged(nameof(IsDataAvailable));
                    //OnPropertyChanged(nameof(IsDataUnavailable));
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        public bool IsDataAvailable => _item != null;
        public bool IsDataUnavailable => !IsDataAvailable;

        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        public ICommand BackCommand => new RelayCommand(OnBack);
        virtual protected void OnBack()
        {
            StatusReady();
            if (navigationService.CanGoBack)
            {
                navigationService.GoBack();
            }
        }

        public ICommand EditCommand => new RelayCommand(OnEdit);
        virtual protected void OnEdit()
        {
            StatusReady();
            BeginEdit();
            //MessageService.Send(this, "BeginEdit", Item);
            Messenger.Send(new ItemMessage<TModel>(Item, "BeginEdit"));
        }
        virtual public void BeginEdit()
        {
            if (!IsEditMode)
            {
                IsEditMode = true;
                // Create a copy for edit
                var editableItem = new TModel();
                //editableItem.Merge(Item);
                EditableItem = editableItem;
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

        public ICommand CancelCommand => new RelayCommand(OnCancel);
        virtual protected void OnCancel()
        {
            StatusReady();
            CancelEdit();
            //MessageService.Send(this, "CancelEdit", Item);
            Messenger.Send(new ItemMessage<TModel>(Item, "CancelEdit"));
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
        virtual protected async void OnSave()
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
        virtual public async Task SaveAsync()
        {
            IsEnabled = false;
            bool isNew = ItemIsNew;
            if (await SaveItemAsync(EditableItem))
            {
                //Item.Merge(EditableItem);
                //Item.NotifyChanges();
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
        virtual protected async void OnDelete()
        {
            StatusReady();
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
                    //MessageService.Send(this, "ItemDeleted", model);
                    Messenger.Send(new ItemMessage<TModel>(model, "ItemDeleted"));
                }
                else
                {
                    IsEnabled = true;
                }
            }
        }

        abstract public bool ItemIsNew { get; }


        public bool CanGoBack => !IsMainView && navigationService.CanGoBack;

        virtual public Result Validate(TModel model)
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
        virtual protected IEnumerable<IValidationConstraint<TModel>> GetValidationConstraints(TModel model) => Enumerable.Empty<IValidationConstraint<TModel>>();

        abstract protected Task<bool> SaveItemAsync(TModel model);
        abstract protected Task<bool> DeleteItemAsync(TModel model);
        abstract protected Task<bool> ConfirmDeleteAsync();
    }
}
