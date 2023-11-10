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

using CommunityToolkit.Mvvm.DependencyInjection;
using Inventory.Uwp.Services;
using Inventory.Uwp.ViewModels.Logs;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Inventory.Uwp.Views.Logs
{
    public sealed partial class LogsView : Page
    {
        private readonly WindowManagerService _windowService;

        public LogsView()
        {
            ViewModel = Ioc.Default.GetService<LogsViewModel>();
            _windowService = Ioc.Default.GetService<WindowManagerService>();
            InitializeComponent();
        }

        public LogsViewModel ViewModel { get; }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.Subscribe();
            await ViewModel.LoadAsync(e.Parameter as LogListArgs);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            ViewModel.Unload();
            ViewModel.Unsubscribe();
        }

        private async void OpenInNewView(object sender, RoutedEventArgs e)
        {
            var args = ViewModel.LogList.CreateArgs();
            args.IsMainView = false;
            await _windowService.OpenWindow(typeof(LogsView), args);
        }

        public int GetRowSpan(bool isMultipleSelection)
        {
            return isMultipleSelection ? 2 : 1;
        }
    }
}
