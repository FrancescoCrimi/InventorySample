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

using Inventory.Infrastructure.Logging;
using Inventory.Uwp.Activation;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace Inventory.Uwp.Services
{
    internal class ActivationService
    {
        private readonly ILogger _logger;
        private readonly ActivationHandler<IActivatedEventArgs> _defaultHandler;
        private readonly IEnumerable<ActivationHandler> _activationHandlers;
        private readonly AppSettings _appSettings;
        private object _lastActivationArgs;

        public ActivationService(ILogger<ActivationService> logger,
                                 ActivationHandler<IActivatedEventArgs> defaultHandler,
                                 IEnumerable<ActivationHandler> activationHandlers,
                                 AppSettings appSettings)
        {
            _logger = logger;
            _defaultHandler = defaultHandler;
            _activationHandlers = activationHandlers;
            _appSettings = appSettings;
        }

        public async Task ActivateAsync(object activationArgs)
        {
            if (IsInteractive(activationArgs))
            {
                await InitializeAsync();
                if (Window.Current.Content == null)
                {
                    Window.Current.Content = new Views.ShellPage();
                }
            }

            await HandleActivationAsync(activationArgs);
            _lastActivationArgs = activationArgs;

            if (IsInteractive(activationArgs))
            {
                Window.Current.Activate();
                await StartupAsync();
            }
        }

        private async Task InitializeAsync()
        {
            ThemeSelectorService.Initialize();
            //await WindowManagerService.Current.InitializeAsync();
            await _appSettings.EnsureLogDatabaseAsync();
            await _appSettings.EnsureLocalDatabaseAsync();
            _logger.LogInformation(LogEvents.Startup, "Application Started");
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
    }
}
