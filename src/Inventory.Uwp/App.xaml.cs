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
using Inventory.Infrastructure.Logging;
using Inventory.Interface.Services;
using Inventory.Uwp.Services;
using Inventory.Uwp.Views.Dashboard;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace Inventory.Uwp
{
    public sealed partial class App : Windows.UI.Xaml.Application
    {
        private readonly ILogger _logger;

        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
            UnhandledException += OnUnhandledException;

            var serviceCollection = new ServiceCollection();
            CompositionRoot.AddAllService(serviceCollection);

            Ioc.Default.ConfigureServices(serviceCollection.BuildServiceProvider());
            _logger = Ioc.Default.GetService<ILogger<App>>();
        }

        protected async override void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (!args.PrelaunchActivated)
            {
                await ActivateAsync(args);
            }
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            await ActivateAsync(args);
        }

        public async Task ActivateAsync(IActivatedEventArgs activationArgs)
        {
            await InitializeAsync();
            if (Window.Current.Content == null)
            {
                Window.Current.Content = new Views.ShellPage();
            }

            object arguments = null;
            if (activationArgs is LaunchActivatedEventArgs launchArgs)
            {
                arguments = launchArgs.Arguments;
            }
            Ioc.Default.GetService<NavigationService>().Navigate(typeof(DashboardView), arguments);

            Window.Current.Activate();
            await StartupAsync();
        }

        private async Task InitializeAsync()
        {
            ThemeSelectorService.Initialize();

            //var appSettings = Ioc.Default.GetService<AppSettings>();
            //await appSettings.EnsureLogDatabaseAsync();
            //await appSettings.EnsureLocalDatabaseAsync();

            var sppBootstrap = Ioc.Default.GetService<AppBootstrapper>();
            await sppBootstrap.InitializeAsync();

            await Ioc.Default.GetService<LookupTablesServiceFacade>().InitializeAsync();
            _logger.LogInformation(LogEvents.Startup, "Application Started");
        }

        private async Task StartupAsync()
        {
            await ThemeSelectorService.SetRequestedThemeAsync();
        }


        private void OnSuspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            _logger.LogInformation(LogEvents.Suspending, $"Application ended.");
        }

        private void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            _logger.LogError(LogEvents.UnhandledException, e.Exception, "Unhandled Exception");
        }
    }
}
