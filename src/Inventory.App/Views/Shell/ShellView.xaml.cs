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

using CiccioSoft.Inventory.Services;
using CiccioSoft.Inventory.ViewModels;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CiccioSoft.Inventory.Views
{
    public sealed partial class ShellView : Page
    {
        public ShellView()
        {
            ViewModel = Ioc.Default.GetService<ShellViewModel>();
            InitializeContext();
            InitializeComponent();
            InitializeNavigation();
        }

        public ShellViewModel ViewModel { get; private set; }

        private void InitializeContext()
        {
            //var context = Ioc.Default.GetService<IContextService>();
            //context.Initialize(Dispatcher, ApplicationView.GetForCurrentView().Id, CoreApplication.GetCurrentView().IsMain);
        }

        private void InitializeNavigation()
        {
            var navigationService = Ioc.Default.GetService<INavigationService>();
            navigationService.Initialize(frame);
            var appView = ApplicationView.GetForCurrentView();
            appView.Consolidated += OnViewConsolidated;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await ViewModel.LoadAsync(e.Parameter as ShellArgs);
            ViewModel.Subscribe();
        }

        private void OnViewConsolidated(ApplicationView sender, ApplicationViewConsolidatedEventArgs args)
        {
            ViewModel.Unsubscribe();
            ViewModel = null;
            Bindings.StopTracking();
            var appView = ApplicationView.GetForCurrentView();
            appView.Consolidated -= OnViewConsolidated;
            //ServiceLocator.DisposeCurrent();
        }

        private  void OnUnlockClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //var context = Ioc.Default.GetService<IContextService>();
            //await ApplicationViewSwitcher.SwitchAsync(context.MainViewID);
        }
    }
}
