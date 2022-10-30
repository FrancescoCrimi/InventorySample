using CommunityToolkit.Mvvm.DependencyInjection;
using Inventory.Uwp.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Inventory.Uwp.Views
{
    public sealed partial class TwoPaneViewPage : Page
    {
        public TwoPaneViewViewModel ViewModel { get; } = Ioc.Default.GetService<TwoPaneViewViewModel>();

        public TwoPaneViewPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
            ViewModel.Initialize(twoPaneView);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await ViewModel.LoadDataAsync();
        }
    }
}
