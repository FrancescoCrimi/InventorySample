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
    /// Collezione Virtuale
    /// 
    /// per funzionare correttamente impostare la Proprietà CacheLength dell'ItemStackPanel a 0.0 cosi
    ///
    ///     <ListView.ItemsPanel>
    ///       <ItemsPanelTemplate>
    ///         <ItemsStackPanel Orientation = "Vertical" CacheLength="0.0"/>
    ///       </ItemsPanelTemplate>
    ///     </ListView.ItemsPanel>
    ///     
    /// Per usare la classe subclassa questa classe implementando i metodi astratti
    /// </summary>
    public abstract class VirtualRangeCollection<T> : IVirtualRangeCollection<T> where T : class
    {
        private readonly ILogger _logger;
        protected readonly CoreDispatcher dispatcher;
        private CancellationTokenSource cancellationTokenSource;
        private readonly IDictionary<int, T> items;
        private readonly List<T> fakelist;
        private readonly T dummyObject;
        private int count = 0;
        private int FirstIndex;
        private int LastIndex;
        private int Length;
        private const string CountString = "Count";
        private const string IndexerName = "Item[]";

        public VirtualRangeCollection(ILogger logger)
        {
            _logger = logger;
            dispatcher = Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().Dispatcher;
            cancellationTokenSource = new CancellationTokenSource();
            items = new ConcurrentDictionary<int, T>();
            fakelist = new List<T>();
            dummyObject = CreateDummyEntity();
            FirstIndex = 0;
            LastIndex = 0;
        }

        protected async Task LoadAsync()
        {
            count = await GetCountAsync();
            if (Length > 0)
            {
                var lengthToFetch = Length * 3;
                FirstIndex = 0;
                LastIndex = 0 + lengthToFetch - 1;

                // recupero i dati
                _logger.LogDebug("Init: {0} - {1}", 0, lengthToFetch - 1);
                var models = await GetRangeAsync(0, lengthToFetch, NewToken());

                // Aggiorno lista interna
                items.Clear();
                for (var i = 0; i < models.Count; i++)
                {
                    items.Add(i, models[i]);
                }
            }

            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(CountString));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(IndexerName));
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            });
        }


        #region abstract method

        //public abstract Task LoadAsync(string searchString = "");
        protected abstract T CreateDummyEntity();
        protected abstract Task<int> GetCountAsync();
        protected abstract Task<IList<T>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken);

        #endregion


        #region private method

        private CancellationToken NewToken()
        {
            if (cancellationTokenSource.Token.CanBeCanceled)
                cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            cancellationTokenSource = new CancellationTokenSource();
            return cancellationTokenSource.Token;
        }

        private async Task FetchRange(int skip, int take, CancellationToken token)
        {
            try
            {
                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();

                // ritardo inserito per velocizzare scrolling
                await Task.Delay(50, token);

                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();

                // recupero i dati
                _logger.LogDebug("FetchRange: {0} - {1}", skip, skip + take - 1);
                var models = await GetRangeAsync(skip, take, token);

                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();

                // Aggiorno lista interna
                items.Clear();
                for (var i = 0; i < models.Count; i++)
                {
                    items.Add(skip + i, models[i]);
                }

                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();

                // invoco CollectionChanged Replace per singolo item
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    try
                    {
                        foreach (var item in items)
                        {
                            if (token.IsCancellationRequested)
                                token.ThrowIfCancellationRequested();
                            var eventArgs = new NotifyCollectionChangedEventArgs(
                                NotifyCollectionChangedAction.Replace,
                                item.Value,
                                null,
                                item.Key);
                            CollectionChanged?.Invoke(this, eventArgs);
                        }
                    }
                    catch (OperationCanceledException ocex)
                    {
                        _logger.LogDebug("NotifyCollectionChanged Replace: {0} - {1} {2}", skip, skip + take - 1, ocex.Message);
                    }
                });
            }
            catch (TaskCanceledException tcex)
            {
                _logger.LogDebug("FetchRange: {0} - {1} {2} Id:{3}", skip, skip + take - 1, tcex.Message, tcex.Task.Id);
            }
            catch (OperationCanceledException ocex)
            {
                _logger.LogDebug(ocex.Message);
            }
        }

        #endregion


        #region interface member implemented

        public void RangesChanged(ItemIndexRange visibleRange, IReadOnlyList<ItemIndexRange> trackedItems)
        {
            var firstVisible = visibleRange.FirstIndex;
            var lastVisible = visibleRange.LastIndex;
            var lengthVisible = (int)visibleRange.Length;
            _logger.LogDebug("VisibleRange: {0} - {1}", firstVisible, lastVisible);

            // se visibleRangeLength è minore di 2 esci
            if (lengthVisible < 2) return;

            // verifico se il range visibile rientra nel range già fetchato
            if (firstVisible < FirstIndex || lastVisible > LastIndex)
            {
                // trovo la lunghezza totale di righe da estrarre
                var lengthToFetch = lengthVisible * 3;

                // prima riga da estrarre
                int firstToFetch;

                // il range si trova all'inizio
                if (firstVisible < lengthVisible * 1)
                    firstToFetch = 0;

                // il range si trova alla fine
                else if (firstVisible >= count - lengthVisible * 2)
                    firstToFetch = count - lengthToFetch;

                // il range si trova nel mezzo
                else
                    firstToFetch = firstVisible - lengthVisible * 1;

                //valorizzo variabli globali firstindex e lastindex;
                FirstIndex = firstToFetch;
                LastIndex = firstToFetch + lengthToFetch - 1;
                Length = lengthVisible;
                var token = NewToken();
                Task.Run(async () => await FetchRange(firstToFetch, lengthToFetch, token), token);
            }
        }

        public T this[int index]
        {
            get
            {
                if (items.ContainsKey(index))
                {
                    //logger.LogDebug("Indexer get real: {0}", index);
                    return items[index];
                }
                else
                {
                    //logger.LogDebug("Indexer get dummy: {0}", index);
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

        public int Count => count;

        public bool IsReadOnly => true;

        public bool IsFixedSize => false;

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public void Dispose()
        {
            if (cancellationTokenSource.Token.CanBeCanceled)
                cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => fakelist.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IList)fakelist).GetEnumerator();
        int IList<T>.IndexOf(T item) => -1;
        int IList.IndexOf(object value) => -1;

        #endregion


        #region interface member not implemented

        bool ICollection.IsSynchronized => throw new NotImplementedException();

        object ICollection.SyncRoot => throw new NotImplementedException();

        void ICollection<T>.Add(T item) => throw new NotImplementedException();
        int IList.Add(object value) => throw new NotImplementedException();
        void ICollection<T>.Clear() => throw new NotImplementedException();
        void IList.Clear() => throw new NotImplementedException();
        bool ICollection<T>.Contains(T item) => throw new NotImplementedException();
        bool IList.Contains(object value) => throw new NotImplementedException();
        void ICollection<T>.CopyTo(T[] array, int arrayIndex) => throw new NotImplementedException();
        void ICollection.CopyTo(Array array, int index) => throw new NotImplementedException();
        void IList<T>.Insert(int index, T item) => throw new NotImplementedException();
        void IList.Insert(int index, object value) => throw new NotImplementedException();
        bool ICollection<T>.Remove(T item) => throw new NotImplementedException();
        void IList.Remove(object value) => throw new NotImplementedException();
        void IList<T>.RemoveAt(int index) => throw new NotImplementedException();
        void IList.RemoveAt(int index) => throw new NotImplementedException();

        #endregion
    }
}
