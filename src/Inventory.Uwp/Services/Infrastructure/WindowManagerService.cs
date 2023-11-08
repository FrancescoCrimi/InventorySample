// Copyright (c) 2023 Francesco Crimi francrim@gmail.com
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.

using Inventory.Uwp.Contracts.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace Inventory.Uwp.Services
{
    public class WindowManagerService : IWindowManagerService, IDisposable
    {
        private readonly ILogger<WindowManagerService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly Dictionary<UIContext, AppWindow> _appWindows;
        private IServiceScope _scope;
        private AppWindow _appWindow;
        private Frame _frame;
        private bool _disposedValue;

        public WindowManagerService(ILogger<WindowManagerService> logger,
                                    IServiceProvider serviceProvider,
                                    IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _serviceScopeFactory = serviceScopeFactory;
            _appWindows = new Dictionary<UIContext, AppWindow>();
        }

        public async Task CloseAllWindows()
        {
            foreach (var pair in _appWindows.ToList())
            {
                await pair.Value.CloseAsync();
            }
            //System.GC.Collect();
        }

        public async Task CloseWindow()
        {
            ////    //int currentId = ApplicationView.GetForCurrentView().Id;
            ////    //await ApplicationViewSwitcher.SwitchAsync(MainViewId, currentId, ApplicationViewSwitchingOptions.ConsolidateViews);
            //await Task.CompletedTask;
            if (_appWindow != null)
            {
                await _appWindow.CloseAsync();
            }
            else
            {
                await ApplicationView.GetForCurrentView().TryConsolidateAsync().AsTask();
            }
        }

        public async Task OpenDialog(string title,
                                     Exception ex,
                                     string ok = "Ok")
        {
            await OpenDialog(title, ex.Message, ok, null);
        }

        public async Task<bool> OpenDialog(string title,
                                           string content,
                                           string ok = "Ok",
                                           string cancel = null)
        {
            var dialog = new ContentDialog
            {
                Title = title,
                Content = content,
                PrimaryButtonText = ok
            };

            if (cancel != null)
            {
                dialog.SecondaryButtonText = cancel;
            }

            if (_appWindow != null)
            {
                dialog.XamlRoot = _frame.XamlRoot;
            }
            else
            {
                dialog.XamlRoot = Window.Current.Content.XamlRoot;
            }

            var result = await dialog.ShowAsync();
            return result == ContentDialogResult.Primary;
        }

        public async Task OpenWindow(Type viewType,
                                     object parameter = null,
                                     string windowTitle = "")
        {
            var scope = _serviceScopeFactory.CreateScope();
            var wms = scope.ServiceProvider.GetRequiredService<WindowManagerService>();
            await wms.OpenWindow(scope, viewType, parameter, windowTitle);
        }


        private async Task OpenWindow(IServiceScope scope,
                                      Type viewType,
                                      object parameter = null,
                                      string windowTitle = "")
        {
            _scope = scope;

            _appWindow = await AppWindow.TryCreateAsync();
            _appWindow.Title = windowTitle;
            _frame = new Frame();

            var navigationService = _serviceProvider.GetService<NavigationService>();
            navigationService.Initialize(_frame);

            ElementCompositionPreview.SetAppWindowContent(_appWindow, _frame);
            _appWindows.Add(_frame.UIContext, _appWindow);
            _appWindow.Closed += AppWindow_Closed;

            navigationService.Navigate(viewType, parameter);

            await _appWindow.TryShowAsync();
        }

        private void AppWindow_Closed(AppWindow sender, AppWindowClosedEventArgs args)
        {
            _appWindow.Closed -= AppWindow_Closed;
            _appWindows.Remove(_frame.UIContext);
            _frame.Content = null;
            _appWindow = null;

            _scope.Dispose();
            _scope = null;
        }

        #region dispose

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: eliminare lo stato gestito (oggetti gestiti)
                }

                // TODO: liberare risorse non gestite (oggetti non gestiti) ed eseguire l'override del finalizzatore
                // TODO: impostare campi di grandi dimensioni su Null
                _disposedValue = true;
            }
        }

        // // TODO: eseguire l'override del finalizzatore solo se 'Dispose(bool disposing)' contiene codice per liberare risorse non gestite
        // ~WindowManagerService()
        // {
        //     // Non modificare questo codice. Inserire il codice di pulizia nel metodo 'Dispose(bool disposing)'
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Non modificare questo codice. Inserire il codice di pulizia nel metodo 'Dispose(bool disposing)'
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion

    }
}
