using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Inventory.Uwp.Services
{
    public class PageService
    {
        private readonly ConcurrentDictionary<Type, Type> _viewModelMap = new ConcurrentDictionary<Type, Type>();

        public PageService()
        {
            ////Register<ShellViewModel, ShellView>();
            //Register<DashboardViewModel, DashboardView>();
            //Register<CustomersViewModel, CustomersView>();
            //Register<CustomerDetailsViewModel, CustomerView>();
            //Register<OrdersViewModel, OrdersView>();
            //Register<OrderDetailsViewModel, OrderView>();
            //Register<OrderItemsViewModel, OrderItemsView>();
            //Register<OrderItemDetailsViewModel, OrderItemView>();
            //Register<ProductsViewModel, ProductsView>();
            //Register<ProductDetailsViewModel, ProductView>();
            //Register<LogsViewModel, LogsView>();
            //Register<SettingsViewModel, SettingsView>();
        }

        public void Register<TViewModel, TView>()
            where TViewModel : ObservableObject
            where TView : Page
        {
            if (!_viewModelMap.TryAdd(typeof(TViewModel), typeof(TView)))
            {
                throw new InvalidOperationException($"ViewModel already registered '{typeof(TViewModel).FullName}'");
            }
        }

        public Type GetView<TViewModel>()
        {
            return GetView(typeof(TViewModel));
        }
        public Type GetView(Type viewModel)
        {
            if (_viewModelMap.TryGetValue(viewModel, out Type view))
            {
                return view;
            }
            throw new InvalidOperationException($"View not registered for ViewModel '{viewModel.FullName}'");
        }

        public Type GetViewModel(Type view)
        {
            var type = _viewModelMap.Where(r => r.Value == view).Select(r => r.Key).FirstOrDefault();
            if (type == null)
            {
                throw new InvalidOperationException($"View not registered for ViewModel '{view.FullName}'");
            }
            return type;
        }

    }
}
