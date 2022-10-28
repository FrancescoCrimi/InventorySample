using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.DependencyInjection;
using Inventory.UwpApp.Activation;

using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Inventory.UwpApp.Services
{
    // For more information on understanding and extending activation flow see
    // https://github.com/microsoft/TemplateStudio/blob/main/docs/UWP/activation.md
    internal class ActivationService
    {
        private readonly ActivationHandler<LaunchActivatedEventArgs> _defaultHandler;
        private readonly IEnumerable<ActivationHandler> _activationHandlers;
        //private Lazy<UIElement> _shell;

        private object _lastActivationArgs;

        public ActivationService(ActivationHandler<LaunchActivatedEventArgs> defaultHandler,
                                 IEnumerable<ActivationHandler> activationHandlers)
        {
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
                    //_shell = Ioc.Default.GetService<Views.ShellPage>();
                    Window.Current.Content = new Views.ShellPage() /*?? new Frame()*/;
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
            await ThemeSelectorService.InitializeAsync().ConfigureAwait(false);
            await WindowManagerService.Current.InitializeAsync();
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
            await ConfigureLookupTables();
        }

        //private IEnumerable<ActivationHandler> GetActivationHandlers()
        //{
        //    yield break;
        //}

        private bool IsInteractive(object args)
        {
            return args is IActivatedEventArgs;
        }

        private async Task ConfigureLookupTables()
        {
            var lookupTables = Ioc.Default.GetService<LookupTableServiceFacade>();
            await lookupTables.InitializeAsync();
            //LookupTablesProxy.Instance = lookupTables;
        }
    }
}
