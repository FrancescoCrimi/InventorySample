using System;

namespace Inventory.Uwp.Helpers
{
    public interface IBackNavigationHandler
    {
        event EventHandler<bool> OnPageCanGoBackChanged;

        void GoBack();
    }
}
