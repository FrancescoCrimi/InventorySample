using CiccioSoft.Inventory.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace CiccioSoft.Inventory.Uwp.Services
{
    public abstract class VirtualRangeCollection<T> : IList<T>, IList, INotifyCollectionChanged where T : class
    {
        //private readonly DbSet<T> dbSet;
        private readonly T dummyModel;
        private readonly List<T> items;
        private readonly int rangeSize;
        private int lastIdxIndexer = 0;
        int intskip;
        private readonly object _sync = new object();
        private bool _isBusy = false;

        public VirtualRangeCollection(/*DbSet<T> dbSet,*/ int rangeSize = 14)
        {
            //this.dbSet = dbSet;
            this.rangeSize = rangeSize;
            dummyModel = CreateDummyEntity();
            items = new List<T>();
            for (int i = 0; i < rangeSize * 2; i++)
            {
                items.Add(dummyModel);
            }

            //Count = dbSet.Count();
            //Task.Run(async () => Count = await GetCountAsync());

            //Task.Run(async () => await FetchData());
            //Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => FetchData());
        }


        private event NotifyCollectionChangedEventHandler collectionChanged;
        event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
        {
            add
            {
                collectionChanged += value;
            }

            remove
            {
                collectionChanged -= value;
            }
        }

        protected abstract T CreateDummyEntity();
        protected abstract Task<IList<T>> FetchRowsAsync(int intskip, int size);
        protected abstract Task<int> GetCountAsync();
        protected async Task LoadAsync()
        {
            await Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().Dispatcher.RunAsync(
                Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    Count = await GetCountAsync();
                });
           
            await Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().Dispatcher.RunAsync(
                Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    await FetchData();
                });
        }

        private async Task FetchData()
        {
            lock (_sync)
            {
                if (_isBusy)
                {
                    return;
                }
                _isBusy = true;
            }

            int cazzo = lastIdxIndexer;
            if (cazzo < rangeSize)
                intskip = 0;
            else
                intskip = cazzo - rangeSize + 1;

            //var query = dbSet.AsQueryable();
            //query = query.Skip(intskip);
            //query = query.Take(rangeSize * 2);
            //IList<T> models = await query.ToListAsync();
            IList<T> models = await FetchRowsAsync(intskip, rangeSize * 2);

            for (int i = 0; i < models.Count; i++)
            {
                var newmodel = models[i];
                items[i] = newmodel;
                collectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
                                                                                     newmodel,
                                                                                     dummyModel,
                                                                                     intskip + i));
            }

            lock (_sync)
            {
                _isBusy = false;
            }
        }



        public T this[int index]
        {
            get
            {
                lastIdxIndexer = index;
                if (index >= intskip && index < intskip + (rangeSize * 2))
                {
                    return items[index - intskip];
                }
                else
                {
                    //App.Current.Dispatcher.Invoke(() => FetchData());                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                
                    Task.Run(async () => await FetchData());
                    return dummyModel;
                }
            }
            set => throw new NotImplementedException();
        }

        object IList.this[int index]
        {
            get => this[index];
            set => throw new NotImplementedException();
        }

        public int Count { get; private set; }

        public bool IsReadOnly => false;

        public bool IsFixedSize => false;

        public IEnumerator<T> GetEnumerator()
        {
            return new List<T>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int IndexOf(T item)
        {
            int idx = items.IndexOf(item);
            return idx + intskip;
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((T)value);
        }

        #region Not Implemented

        bool ICollection.IsSynchronized => throw new NotImplementedException();

        object ICollection.SyncRoot => throw new NotImplementedException();

        void ICollection<T>.Add(T item)
        {
            throw new NotImplementedException();
        }

        int IList.Add(object value)
        {
            throw new NotImplementedException();
        }

        void ICollection<T>.Clear()
        {
            throw new NotImplementedException();
        }

        void IList.Clear()
        {
            throw new NotImplementedException();
        }

        bool ICollection<T>.Contains(T item)
        {
            throw new NotImplementedException();
        }

        bool IList.Contains(object value)
        {
            throw new NotImplementedException();
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        void ICollection.CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        void IList<T>.Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        void IList.Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotImplementedException();
        }

        void IList.Remove(object value)
        {
            throw new NotImplementedException();
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        void IList.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
