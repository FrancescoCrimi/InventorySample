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

using CiccioSoft.Inventory.Uwp.Services;
using CiccioSoft.Inventory.Uwp.Services.Infrastructure;
using CiccioSoft.Inventory.Uwp.ViewModels;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CiccioSoft.Inventory.Uwp.Views
{
    public sealed partial class MainShellView : Page
    {
        private readonly MainShellViewModel viewModel;

        public MainShellView()
        {
            InitializeComponent();
            viewModel = Ioc.Default.GetService<MainShellViewModel>();
            DataContext = viewModel;
            INavigationService navigationService = Ioc.Default.GetService<INavigationService>();
            navigationService.Initialize(frame);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await viewModel.LoadAsync(e.Parameter as ShellArgs);
            viewModel.Subscribe();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            viewModel.Unload();
            viewModel.Unsubscribe();
        }

        private async void OnLogoff(object sender, RoutedEventArgs e)
        {
            var dialogService = Ioc.Default.GetService<IDialogService>();
            if (await dialogService.ShowAsync("Confirm logoff", "Are you sure you want to logoff?", "Ok", "Cancel"))
            {
                //var loginService = Ioc.Default.GetService<ILoginService>();
                //loginService.Logoff();
                if (Frame.CanGoBack)
                {
                    Frame.GoBack();
                }
            }
        }
    }
}
