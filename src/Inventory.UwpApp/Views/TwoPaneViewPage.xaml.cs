using CommunityToolkit.Mvvm.DependencyInjection;
using Inventory.UwpApp.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Inventory.UwpApp.Views
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
