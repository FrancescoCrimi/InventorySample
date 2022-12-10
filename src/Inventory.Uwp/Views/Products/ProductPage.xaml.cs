using CommunityToolkit.Mvvm.DependencyInjection;
using Inventory.Uwp.ViewModels.Products;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Inventory.Uwp.Views.Products
{
    public sealed partial class ProductPage : Page
    {
        public ProductPage()
        {
            ViewModel = Ioc.Default.GetService<ProductDetailsViewModel>();
            InitializeComponent();
        }

        public ProductDetailsViewModel ViewModel { get; }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ViewModel.Unload();
            ViewModel.Unsubscribe();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.Subscribe();
            await ViewModel.LoadAsync(e.Parameter as ProductDetailsArgs);

            if (ViewModel.IsEditMode)
            {
                await Task.Delay(100);
                details.SetFocus();
            }
        }
    }
}
