using System;
using System.Collections.Concurrent;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace CiccioSoft.Inventory.Uwp.Services.Infrastructure
{
    public class PageService
    {
        private readonly ConcurrentDictionary<Type, Type> _viewModelMap = new ConcurrentDictionary<Type, Type>();

        public void Register<TViewModel, TView>() where TView : Page
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
