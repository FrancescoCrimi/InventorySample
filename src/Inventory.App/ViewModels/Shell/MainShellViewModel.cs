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
using CiccioSoft.Inventory.Infrastructure.Common;
using CiccioSoft.Inventory.Uwp.Helpers;
using CiccioSoft.Inventory.Uwp.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using WinUI = Microsoft.UI.Xaml.Controls;

namespace CiccioSoft.Inventory.Uwp.ViewModels
{
    public class MainShellViewModel : ShellViewModel
    {
        private readonly ILogger<MainShellViewModel> logger;
        private readonly INavigationService navigationService;
        private readonly ILogService logService;

        public MainShellViewModel(ILogger<MainShellViewModel> logger,
                                  INavigationService navigationService,
                                  ILogService logService)
            : base(navigationService)
        {
            this.logger = logger;
            this.navigationService = navigationService;
            this.logService = logService;
        }

        public override async Task LoadAsync(ShellArgs args)
        {
            //Items = GetItems().ToArray();
            await UpdateAppLogBadge();
            await base.LoadAsync(args);
        }

        override public void Subscribe()
        {
            // Todo: ILoggerService non scrive piu i log, ma vengono scritti da NLog
            //MessageService.Subscribe<ILogService, Log>(this, OnLogServiceMessage);
            base.Subscribe();
        }

        override public void Unsubscribe()
        {
            base.Unsubscribe();
        }

        public override void Unload()
        {
            base.Unload();
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
