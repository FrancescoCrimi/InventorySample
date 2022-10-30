using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Data;
using System.ComponentModel;

namespace Inventory.Uwp.Library.Common
{
    /// <summary>
    /// Collezione Virtuale che usa l'interfaccia IItemsRangeInfo
    /// </summary>
    public abstract class VirtualRangeCollection<T> : IList<T>, IList, INotifyCollectionChanged, INotifyPropertyChanged, IItemsRangeInfo where T : class
    {
        private readonly ILogger logger;
        protected readonly CoreDispatcher dispatcher;
        private CancellationTokenSource cancellationTokenSource = null;
        private readonly IDictionary<int, T> items;
        private readonly List<T> fakelist;
        private readonly T dummyObject;
        private int count = 0;
        private readonly int cacheLength;
        private int FirstIndex;
        private int LastIndex;
        private const string CountString = "Count";
        private const string IndexerName = "Item[]";

        public VirtualRangeCollection()
        {
            logger = Ioc.Default.GetRequiredService<ILoggerFactory>().CreateLogger("VirtualRangeCollection");
            dispatcher = Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().Dispatcher;
            cancellationTokenSource = new CancellationTokenSource();
            items = new ConcurrentDictionary<int, T>();
            fakelist = new List<T>();
            dummyObject = CreateDummyEntity();
            cacheLength = 1;
            FirstIndex = 0;
            LastIndex = 0;
        }

        public async Task LoadAsync()
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                count = await GetCountAsync();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(CountString));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(IndexerName));
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            });
        }


        #region abstract method

        protected abstract T CreateDummyEntity();
        protected abstract Task<int> GetCountAsync();
        protected abstract Task<IList<T>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken);

        #endregion


        #region private method

        private async Task FetchRange(int skip, int take, CancellationToken token)
        {
            try
            {
                // ritardo inserito per velocizzare scrolling
                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();
                await Task.Delay(60, token);

                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();
                logger.LogWarning("FetchRange First: {0} Length: {1}", skip, take);
                var list = await GetRangeAsync(skip, take, token);

                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();
                items.Clear();
                for (int i = 0; i < list.Count; i++)
                {
                    items.Add(skip + i, list[i]);
                }

                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    foreach (var item in items)
                    {
                        var eventArgs = new NotifyCollectionChangedEventArgs(
                            NotifyCollectionChangedAction.Replace,
                            item.Value,
                            null,
                            item.Key);
                        CollectionChanged?.Invoke(this, eventArgs);
                        logger.LogWarning("CollectionChanged Replace: {0}", item.Key);
                    }
                });
            }
            catch (OperationCanceledException ex)
            {
                logger.LogError(ex.Message);
            }
            catch (AggregateException agex)
            {
                logger.LogError("Cancel Task Id:{0}", ((TaskCanceledException)agex.InnerException).Task.Id);
            }
        }

        private CancellationToken NewToken()
        {
            if (cancellationTokenSource.Token.CanBeCanceled)
                cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            cancellationTokenSource = new CancellationTokenSource();
            return cancellationTokenSource.Token;
        }

        #endregion


        #region interface member implemented

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public void RangesChanged(ItemIndexRange visibleRange, IReadOnlyList<ItemIndexRange> trackedItems)
        {
            int firstVisible = visibleRange.FirstIndex;
            int lastVisible = visibleRange.LastIndex;
            int lengthVisible = (int)visibleRange.Length;
            logger.LogWarning("VisibleRange First: {0} Length: {1}", firstVisible, lengthVisible);

            // se visibleRangeLength è minore di 2 esci
            if (lengthVisible < 2) return;

            // verifico se il range visibile rientra nel range già fetchato
            if (firstVisible < FirstIndex || lastVisible > LastIndex)
            {
                // trovo la lunghezza totale di righe da estrarre
                int lengthToFetch = lengthVisible + (lengthVisible * cacheLength) * 2;

                // prima riga da estrarre
                int firstToFetch;

                // se mi trovo all'inizio della collezione e trovo firstToFetch
                if (firstVisible < lengthVisible * cacheLength)
                    firstToFetch = 0;

                // se mi trovo alla fine della collezione e trovo firstToFetch
                else if (firstVisible > count - (lengthVisible + lengthVisible * cacheLength))
                    firstToFetch = count - lengthToFetch;

                // se mi trovo nel mezzo della collezione e trovo firstToFetch
                else
                    firstToFetch = firstVisible - lengthVisible * cacheLength;

                //valorizzo variabli globali firstindex e lastindex;
                FirstIndex = firstToFetch;
                LastIndex = firstToFetch + lengthToFetch - 1;

                Task.Run(async () => await FetchRange(firstToFetch, lengthToFetch, NewToken()));
            }
        }

        public T this[int index]
        {
            get
            {
                if (items.ContainsKey(index))
                {
                    //logger.LogWarning("Indexer get real: {0}", index);
                    return items[index];
                }
                else
                {
                    //logger.LogWarning("Indexer get dummy: {0}", index);
                    return dummyObject;
                }
            }
            set => throw new NotImplementedException();
        }

        object IList.this[int index]
        {
            get => this[index];
            set => throw new NotImplementedException();
        }

        public int Count
        {
            get
            {
                return count;
            }
        }

        public int IndexOf(T item) => -1;

        int IList.IndexOf(object value) => -1;

        public IEnumerator<T> GetEnumerator() => fakelist.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IList)fakelist).GetEnumerator();

        public bool IsReadOnly => true;

        bool IList.IsFixedSize => false;

        public void Dispose()
        {
            if (cancellationTokenSource.Token.CanBeCanceled)
                cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
        }

        #endregion


        #region interface member not implemented

        bool ICollection.IsSynchronized => throw new NotImplementedException();

        object ICollection.SyncRoot => throw new NotImplementedException();

        int IList.Add(object value) => throw new NotImplementedException();

        public void Clear() => throw new NotImplementedException();

        bool IList.Contains(object value) => throw new NotImplementedException();

        void ICollection.CopyTo(Array array, int index) => throw new NotImplementedException();

        void IList.Insert(int index, object value) => throw new NotImplementedException();

        void IList.Remove(object value) => throw new NotImplementedException();

        public void RemoveAt(int index) => throw new NotImplementedException();

        void IList<T>.Insert(int index, T item) => throw new NotImplementedException();

        void ICollection<T>.Add(T item) => throw new NotImplementedException();

        bool ICollection<T>.Contains(T item) => throw new NotImplementedException();

        void ICollection<T>.CopyTo(T[] array, int arrayIndex) => throw new NotImplementedException();

        bool ICollection<T>.Remove(T item) => throw new NotImplementedException();

        #endregion
    }
}
