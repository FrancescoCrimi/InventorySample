using Inventory.Uwp.Services;
using Inventory.Uwp.Views.Dashboard;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace Inventory.Uwp.Activation
{
    internal class DefaultActivationHandler : ActivationHandler<IActivatedEventArgs>
    {
        private readonly NavigationService navigationService;

        public DefaultActivationHandler(NavigationService navigationService)
        {
            this.navigationService = navigationService;
        }

        protected override async Task HandleInternalAsync(IActivatedEventArgs args)
        {
            // When the navigation stack isn't restored, navigate to the first page and configure
            // the new page by passing required information in the navigation parameter
            object arguments = null;
            if (args is LaunchActivatedEventArgs launchArgs)
            {
                arguments = launchArgs.Arguments;
            }
            navigationService.Navigate(typeof(DashboardPage), arguments);
            await Task.CompletedTask;
        }

        protected override bool CanHandleInternal(IActivatedEventArgs args)
        {
            // None of the ActivationHandlers has handled the app activation
            return navigationService.Frame.Content == null;
        }
    }
}
