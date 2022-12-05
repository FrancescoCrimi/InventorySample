using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Inventory.Uwp.Library.Common
{
    public interface IVirtualRangeCollection<T> : IList<T>,
                                                  IList,
                                                  IReadOnlyList<T>,
                                                  INotifyCollectionChanged,
                                                  INotifyPropertyChanged,
                                                  IItemsRangeInfo where T : class
    {
        //Task LoadAsync(string searchString = "");
    }
}
