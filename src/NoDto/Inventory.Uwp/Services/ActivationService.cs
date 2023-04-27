using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.DependencyInjection;
using Inventory.Infrastructure.Logging;
using Inventory.Uwp.Activation;
using Microsoft.Extensions.Logging;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.Xaml;

namespace Inventory.Uwp.Services
{
    // For more information on understanding and extending activation flow see
    // https://github.com/microsoft/TemplateStudio/blob/main/docs/UWP/activation.md
    internal class ActivationService
    {
        private readonly ILogger _logger;
        private readonly ActivationHandler<IActivatedEventArgs> _defaultHandler;
        private readonly IEnumerable<ActivationHandler> _activationHandlers;
        private object _lastActivationArgs;

        public ActivationService(ILogger<ActivationService> logger,
                                 ActivationHandler<IActivatedEventArgs> defaultHandler,
                                 IEnumerable<ActivationHandler> activationHandlers)
        {
            _logger = logger;
            _defaultHandler = defaultHandler;
            _activationHandlers = activationHandlers;
        }

        public async Task ActivateAsync(object activationArgs)
        {
            if (IsInteractive(activationArgs))
            {
                // Initialize services that you need before app activation
                // take into account that the splash screen is shown while this code runs.
                await InitializeAsync();

                // Do not repeat app initialization when the Window already has content,
                // just ensure that the window is active
                if (Window.Current.Content == null)
                {
                    // Create a Shell or Frame to act as the navigation context
                    Window.Current.Content = new Views.ShellPage();
                }
            }

            // Depending on activationArgs one of ActivationHandlers or DefaultActivationHandler
            // will navigate to the first page
            await HandleActivationAsync(activationArgs);
            _lastActivationArgs = activationArgs;

            if (IsInteractive(activationArgs))
            {
                // Ensure the current window is active
                Window.Current.Activate();

                // Tasks after activation
                await StartupAsync();
            }
        }

        private async Task InitializeAsync()
        {
            ThemeSelectorService.Initialize();
            //await WindowManagerService.Current.InitializeAsync();

            await EnsureLogDbAsync();
            await EnsureDatabaseAsync();
            await ConfigureLookupTables();

            //await logService.WriteAsync(Data.LogType.Information, "Startup", "Configuration", "Application Start", $"Application started.");
            _logger.LogInformation(LogEvents.Configuration, "Application Started");
        }

        private async Task HandleActivationAsync(object activationArgs)
        {
            var activationHandler = _activationHandlers
                                                .FirstOrDefault(h => h.CanHandle(activationArgs));

            if (activationHandler != null)
            {
                await activationHandler.HandleAsync(activationArgs);
            }

            if (IsInteractive(activationArgs))
            {
                if (_defaultHandler.CanHandle(activationArgs))
                {
                    await _defaultHandler.HandleAsync(activationArgs);
                }
            }
        }

        private async Task StartupAsync()
        {
            await ThemeSelectorService.SetRequestedThemeAsync();
        }

        private bool IsInteractive(object args)
        {
            return args is IActivatedEventArgs;
        }


        private async Task EnsureLogDbAsync()
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            var appLogFolder = await localFolder.CreateFolderAsync(AppSettings.AppLogPath, CreationCollisionOption.OpenIfExists);
            if (await appLogFolder.TryGetItemAsync(AppSettings.AppLogName) == null)
            {
                var sourceLogFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/AppLog/AppLog.db"));
                var targetLogFile = await appLogFolder.CreateFileAsync(AppSettings.AppLogName, CreationCollisionOption.ReplaceExisting);
                await sourceLogFile.CopyAndReplaceAsync(targetLogFile);
                //CreateSqliteLogDb();
            }
        }

        private async Task ConfigureLookupTables()
        {
            var lookupTables = Ioc.Default.GetService<LookupTablesService>();
            await lookupTables.InitializeAsync();
        }

        private async Task EnsureDatabaseAsync()
        {
            await EnsureSQLiteDatabaseAsync();
        }

        private async Task EnsureSQLiteDatabaseAsync()
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            var databaseFolder = await localFolder.CreateFolderAsync(AppSettings.DatabasePath, CreationCollisionOption.OpenIfExists);
            if (await databaseFolder.TryGetItemAsync(AppSettings.DatabaseName) == null)
            {
                var sourceFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Database/VanArsdel.1.01.db"));
                var targetFile = await databaseFolder.CreateFileAsync(AppSettings.DatabaseName, CreationCollisionOption.ReplaceExisting);
                await sourceFile.CopyAndReplaceAsync(targetFile);
            }
        }
    }
}
