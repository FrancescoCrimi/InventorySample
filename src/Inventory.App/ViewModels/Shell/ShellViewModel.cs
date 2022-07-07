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
using CiccioSoft.Inventory.Data.Services;
using CiccioSoft.Inventory.Infrastructure.Common;
using CiccioSoft.Inventory.Uwp.Helpers;
using CiccioSoft.Inventory.Uwp.Services.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using WinUI = Microsoft.UI.Xaml.Controls;

namespace CiccioSoft.Inventory.Uwp.ViewModels
{
    public class ShellArgs
    {
        public Type ViewModel { get; set; }
        public object Parameter { get; set; }
        public UserInfo UserInfo { get; set; }
    }

    public class ShellViewModel : ViewModelBase
    {
        private readonly ILogger<ShellViewModel> logger;
        private readonly NavigationService navigationService;
        private readonly ILogService logService;

        public ShellViewModel(ILogger<ShellViewModel> logger,
                                  NavigationService navigationService,
                                  ILogService logService)
        {
            this.logger = logger;
            this.navigationService = navigationService;
            this.logService = logService;
        }

        public ShellArgs ViewModelArgs { get; protected set; }

        public UserInfo UserInfo { get; protected set; }

        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        private bool _isError = false;
        public bool IsError
        {
            get => _isError;
            set => SetProperty(ref _isError, value);
        }

        private string _message = "Ready";
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        private int logNewCount = 10;
        public int LogNewCount
        {
            get => logNewCount;
            set => SetProperty(ref logNewCount, value);
        }

        private bool isBackEnabled;
        public bool IsBackEnabled
        {
            get => isBackEnabled;
            set => SetProperty(ref isBackEnabled, value);
        }

        public async Task LoadAsync(ShellArgs args)
        {
            //Items = GetItems().ToArray();
            await UpdateAppLogBadge();
            ViewModelArgs = args;
            if (ViewModelArgs != null)
            {
                UserInfo = ViewModelArgs.UserInfo;
                navigationService.Navigate(ViewModelArgs.ViewModel, ViewModelArgs.Parameter);
            }
        }

        public void Subscribe()
        {
            // Todo: ILoggerService non scrive piu i log, ma vengono scritti da NLog
            //MessageService.Subscribe<ILogService, Log>(this, OnLogServiceMessage);
            //MessageService.Subscribe<ILoginService, bool>(this, OnLoginMessage);
            //MessageService.Subscribe<ViewModelBase, string>(this, OnMessage);
            Messenger.Register<StatusMessage>(this, OnStatusMessage);
        }

        private void SetStatus(string message)
        {
            message = message ?? "";
            message = message.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");
            Message = message;
        }

        private void OnStatusMessage(object recipient, StatusMessage message)
        {
            //    throw new NotImplementedException();
            //}

            //private async void OnMessage(ViewModelBase viewModel, string message, string status)
            //{
            switch (message.Value)
            {
                case "StatusMessage":
                case "StatusError":
                    IsError = message.Value == "StatusError";
                    SetStatus(message.Args);
                    break;

                case "EnableThisView":
                case "DisableThisView":
                    IsEnabled = message.Value == "EnableThisView";
                    SetStatus(message.Args);
                    break;

                case "EnableOtherViews":
                case "DisableOtherViews":
                    IsEnabled = message.Value == "EnableOtherViews";
                    SetStatus(message.Args);
                    break;

                case "EnableAllViews":
                case "DisableAllViews":
                    IsEnabled = message.Value == "EnableAllViews";
                    SetStatus(message.Args);
                    break;
            }
        }

        public void Unsubscribe()
        {
            //MessageService.Unsubscribe(this);
            Messenger.UnregisterAll(this);
        }

        public void Unload()
        {
        }

        private async void NavigateTo(Type viewModel)
        {
            switch (viewModel.Name)
            {
                case "DashboardViewModel":
                    navigationService.Navigate(viewModel);
                    break;
                case "CustomersViewModel":
                    navigationService.Navigate(viewModel, new CustomerListArgs());
                    break;
                case "OrdersViewModel":
                    navigationService.Navigate(viewModel, new OrderListArgs());
                    break;
                case "ProductsViewModel":
                    navigationService.Navigate(viewModel, new ProductListArgs());
                    break;
                case "AppLogsViewModel":
                    navigationService.Navigate(viewModel, new AppLogListArgs());
                    await logService.MarkAllAsReadAsync();
                    await UpdateAppLogBadge();
                    break;
                case "SettingsViewModel":
                    navigationService.Navigate(viewModel, new SettingsArgs());
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        //private async void OnLogServiceMessage(ILogService logService, string message, Log log)
        //{
        //    if (message == "LogAdded")
        //    {
        //        await UpdateAppLogBadge();
        //    }
        //}

        private async Task UpdateAppLogBadge()
        {
            int count = await logService.GetLogsCountAsync(new DataRequest<Log> { /*Where = r => !r.IsRead*/ });
            //AppLogsItem.Badge = count > 0 ? count.ToString() : null;
        }


        private RelayCommand<WinUI.NavigationViewItemInvokedEventArgs> itemInvokedCommand;
        public ICommand ItemInvokedCommand => itemInvokedCommand ??
            (itemInvokedCommand = new RelayCommand<WinUI.NavigationViewItemInvokedEventArgs>(ItemInvoked));
        private void ItemInvoked(NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                NavigateTo(typeof(SettingsViewModel));
            }
            else
            {
                var selectedItem = args.InvokedItemContainer as WinUI.NavigationViewItem;
                var pageType = selectedItem?.GetValue(NavHelper.NavigateToProperty) as Type;

                if (pageType != null)
                {
                    NavigateTo(pageType);
                }
            }
        }

        private RelayCommand<NavigationViewBackRequestedEventArgs> backRequestedCommand;
        public ICommand BackRequestedCommand => backRequestedCommand ??
            (backRequestedCommand = new RelayCommand<NavigationViewBackRequestedEventArgs>(BackRequested));
        private void BackRequested(NavigationViewBackRequestedEventArgs obj)
        {
            if (navigationService.CanGoBack)
            {
                navigationService.GoBack();
            }
        }

        private RelayCommand frameNavigatedCommand;
        public ICommand FrameNavigatedCommand => frameNavigatedCommand ??
            (frameNavigatedCommand = new RelayCommand(FrameNavigated));
        private void FrameNavigated()
        {
            IsBackEnabled = navigationService.CanGoBack;
        }
    }
}
