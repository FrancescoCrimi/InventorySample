using Inventory.Uwp.Services;
using Inventory.Uwp.Views.Dashboard;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace Inventory.Uwp.Activation
{
    internal class DefaultActivationHandler : ActivationHandler<LaunchActivatedEventArgs>
    {
        private readonly NavigationService navigationService;

        public DefaultActivationHandler(NavigationService navigationService)
        {
            this.navigationService = navigationService;
        }

        protected override async Task HandleInternalAsync(LaunchActivatedEventArgs args)
        {
            navigationService.Navigate(typeof(DashboardPage), args.Arguments);
            await Task.CompletedTask;
        }

        protected override bool CanHandleInternal(LaunchActivatedEventArgs args)
        {
            // None of the ActivationHandlers has handled the app activation
            return navigationService.Frame.Content == null;
        }
    }
}
