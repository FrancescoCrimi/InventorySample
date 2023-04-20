#region copyright
// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************
#endregion

using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Inventory.Uwp.Library.Extensions;
using Inventory.Uwp.Library.Tools;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Inventory.Uwp.Library.Controls
{
    public sealed partial class DataGrid : UserControl, INotifyExpressionChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public DataGrid()
        {
            InitializeComponent();
            DependencyExpressions.Initialize(this);
        }

        private static readonly DependencyExpressions DependencyExpressions = new DependencyExpressions();

        #region NewLabel
        public string NewLabel
        {
            get => (string)GetValue(NewLabelProperty);
            set => SetValue(NewLabelProperty, value);
        }

        public static readonly DependencyProperty NewLabelProperty = DependencyProperty.Register(nameof(NewLabel), typeof(string), typeof(DataGrid), new PropertyMetadata("New"));
        #endregion

        #region ItemsSource*
        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        private static void ItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as DataGrid;
            control.UpdateItemsSource(e.NewValue, e.OldValue);
            DependencyExpressions.UpdateDependencies(control, nameof(ItemsSource));
        }

        private void UpdateItemsSource(object newValue, object oldValue)
        {
            if (oldValue is INotifyCollectionChanged oldSource)
            {
                oldSource.CollectionChanged -= OnCollectionChanged;
            }
            if (newValue is INotifyCollectionChanged newSource)
            {
                newSource.CollectionChanged += OnCollectionChanged;
            }
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(DataGrid), new PropertyMetadata(null, ItemsSourceChanged));
        #endregion

        #region HeaderTemplate
        public DataTemplate HeaderTemplate
        {
            get => (DataTemplate)GetValue(HeaderTemplateProperty);
            set => SetValue(HeaderTemplateProperty, value);
        }

        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register(nameof(HeaderTemplate), typeof(DataTemplate), typeof(DataGrid), new PropertyMetadata(null));
        #endregion

        #region ItemTemplate
        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(DataGrid), new PropertyMetadata(null));
        #endregion


        #region SelectedItem
        public object SelectedItem
        {
            get => (object)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(DataGrid), new PropertyMetadata(null));
        #endregion

        #region IsMultipleSelection*
        public bool IsMultipleSelection
        {
            get => (bool)GetValue(IsMultipleSelectionProperty);
            set => SetValue(IsMultipleSelectionProperty, value);
        }

        private static void IsMultipleSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as DataGrid;
            DependencyExpressions.UpdateDependencies(control, nameof(IsMultipleSelection));
        }

        public static readonly DependencyProperty IsMultipleSelectionProperty = DependencyProperty.Register(nameof(IsMultipleSelection), typeof(bool), typeof(DataGrid), new PropertyMetadata(null, IsMultipleSelectionChanged));
        #endregion

        #region SelectedItemsCount*
        public int SelectedItemsCount
        {
            get => (int)GetValue(SelectedItemsCountProperty);
            set => SetValue(SelectedItemsCountProperty, value);
        }

        private static void SelectedItemsCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as DataGrid;
            DependencyExpressions.UpdateDependencies(control, nameof(SelectedItemsCount));
        }

        public static readonly DependencyProperty SelectedItemsCountProperty = DependencyProperty.Register(nameof(SelectedItemsCount), typeof(int), typeof(DataGrid), new PropertyMetadata(null, SelectedItemsCountChanged));
        #endregion


        #region Query
        public string Query
        {
            get => (string)GetValue(QueryProperty);
            set => SetValue(QueryProperty, value);
        }

        public static readonly DependencyProperty QueryProperty = DependencyProperty.Register(nameof(Query), typeof(string), typeof(DataGrid), new PropertyMetadata(null));
        #endregion

        #region ItemsCount
        public int ItemsCount
        {
            get => (int)GetValue(ItemsCountProperty);
            set => SetValue(ItemsCountProperty, value);
        }

        public static readonly DependencyProperty ItemsCountProperty = DependencyProperty.Register(nameof(ItemsCount), typeof(int), typeof(DataGrid), new PropertyMetadata(0));
        #endregion


        #region RefreshCommand
        public ICommand RefreshCommand
        {
            get => (ICommand)GetValue(RefreshCommandProperty);
            set => SetValue(RefreshCommandProperty, value);
        }

        public static readonly DependencyProperty RefreshCommandProperty = DependencyProperty.Register(nameof(RefreshCommand), typeof(ICommand), typeof(DataGrid), new PropertyMetadata(null));
        #endregion

        #region QuerySubmittedCommand
        public ICommand QuerySubmittedCommand
        {
            get => (ICommand)GetValue(QuerySubmittedCommandProperty);
            set => SetValue(QuerySubmittedCommandProperty, value);
        }

        public static readonly DependencyProperty QuerySubmittedCommandProperty = DependencyProperty.Register(nameof(QuerySubmittedCommand), typeof(ICommand), typeof(DataGrid), new PropertyMetadata(null));
        #endregion

        #region NewCommand
        public ICommand NewCommand
        {
            get => (ICommand)GetValue(NewCommandProperty);
            set => SetValue(NewCommandProperty, value);
        }

        public static readonly DependencyProperty NewCommandProperty = DependencyProperty.Register(nameof(NewCommand), typeof(ICommand), typeof(DataGrid), new PropertyMetadata(null));
        #endregion

        #region DeleteCommand
        public ICommand DeleteCommand
        {
            get => (ICommand)GetValue(DeleteCommandProperty);
            set => SetValue(DeleteCommandProperty, value);
        }

        public static readonly DependencyProperty DeleteCommandProperty = DependencyProperty.Register(nameof(DeleteCommand), typeof(ICommand), typeof(DataGrid), new PropertyMetadata(null));
        #endregion


        #region StartSelectionCommand
        public ICommand StartSelectionCommand
        {
            get => (ICommand)GetValue(StartSelectionCommandProperty);
            set => SetValue(StartSelectionCommandProperty, value);
        }

        public static readonly DependencyProperty StartSelectionCommandProperty = DependencyProperty.Register(nameof(StartSelectionCommand), typeof(ICommand), typeof(DataGrid), new PropertyMetadata(null));
        #endregion

        #region CancelSelectionCommand
        public ICommand CancelSelectionCommand
        {
            get => (ICommand)GetValue(CancelSelectionCommandProperty);
            set => SetValue(CancelSelectionCommandProperty, value);
        }

        public static readonly DependencyProperty CancelSelectionCommandProperty = DependencyProperty.Register(nameof(CancelSelectionCommand), typeof(ICommand), typeof(DataGrid), new PropertyMetadata(null));
        #endregion

        #region SelectItemsCommand
        public ICommand SelectItemsCommand
        {
            get => (ICommand)GetValue(SelectItemsCommandProperty);
            set => SetValue(SelectItemsCommandProperty, value);
        }

        public static readonly DependencyProperty SelectItemsCommandProperty = DependencyProperty.Register(nameof(SelectItemsCommand), typeof(ICommand), typeof(DataGrid), new PropertyMetadata(null));
        #endregion

        #region DeselectItemsCommand
        public ICommand DeselectItemsCommand
        {
            get => (ICommand)GetValue(DeselectItemsCommandProperty);
            set => SetValue(DeselectItemsCommandProperty, value);
        }

        public static readonly DependencyProperty DeselectItemsCommandProperty = DependencyProperty.Register(nameof(DeselectItemsCommand), typeof(ICommand), typeof(DataGrid), new PropertyMetadata(null));
        #endregion

        #region SelectRangesCommand
        public ICommand SelectRangesCommand
        {
            get => (ICommand)GetValue(SelectRangesCommandProperty);
            set => SetValue(SelectRangesCommandProperty, value);
        }

        public static readonly DependencyProperty SelectRangesCommandProperty = DependencyProperty.Register(nameof(SelectRangesCommand), typeof(ICommand), typeof(DataGrid), new PropertyMetadata(null));
        #endregion

        #region ItemInvokedCommand
        public ICommand ItemInvokedCommand
        {
            get => (ICommand)GetValue(ItemInvokedCommandProperty);
            set => SetValue(ItemInvokedCommandProperty, value);
        }

        public static readonly DependencyProperty ItemInvokedCommandProperty = DependencyProperty.Register(nameof(ItemInvokedCommand), typeof(ICommand), typeof(DataGrid), new PropertyMetadata(null));
        #endregion

        #region ItemSecondaryActionInvokedCommand
        public ICommand ItemSecondaryActionInvokedCommand
        {
            get => (ICommand)GetValue(ItemSecondaryActionInvokedCommandProperty);
            set => SetValue(ItemSecondaryActionInvokedCommandProperty, value);
        }

        public static readonly DependencyProperty ItemSecondaryActionInvokedCommandProperty = DependencyProperty.Register(nameof(ItemSecondaryActionInvokedCommand), typeof(ICommand), typeof(DataGrid), new PropertyMetadata(null));
        #endregion


        public ListToolbarMode ToolbarMode => IsMultipleSelection ? (SelectedItemsCount > 0 ? ListToolbarMode.CancelDelete : ListToolbarMode.Cancel) : ListToolbarMode.Default;
        public static readonly DependencyExpression ToolbarModeExpression = DependencyExpressions.Register(nameof(ToolbarMode), nameof(IsMultipleSelection), nameof(SelectedItemsCount));

        public ListViewSelectionMode SelectionMode => IsMultipleSelection ? ListViewSelectionMode.Multiple : ListViewSelectionMode.None;
        public static readonly DependencyExpression SelectionModeExpression = DependencyExpressions.Register(nameof(SelectionMode), nameof(IsMultipleSelection));

        public bool IsSingleSelection => !IsMultipleSelection;
        public static readonly DependencyExpression IsSingleSelectionExpression = DependencyExpressions.Register(nameof(IsSingleSelection), nameof(IsMultipleSelection));

        public bool IsDataAvailable
        {
            get
            {
                //var rtn = (ItemsSource?.Cast<object>().Any() ?? false);
                var rtn = ((ICollection)ItemsSource)?.Count != 0;
                return rtn;
            }
        }

        public static readonly DependencyExpression IsDataAvailableExpression = DependencyExpressions.Register(nameof(IsDataAvailable), nameof(ItemsSource));

        public bool IsDataUnavailable => !IsDataAvailable;
        public static readonly DependencyExpression IsDataUnavailableExpression = DependencyExpressions.Register(nameof(IsDataUnavailable), nameof(IsDataAvailable));

        public string DataUnavailableMessage => ItemsSource == null ? "Loading..." : "No items found.";
        public static readonly DependencyExpression DataUnavailableMessageExpression = DependencyExpressions.Register(nameof(DataUnavailableMessage), nameof(ItemsSource));

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!IsMultipleSelection)
            {
                if (ItemsSource is IList list)
                {
                    if (e.Action == NotifyCollectionChangedAction.Replace)
                    {
                        if (ItemsSource is ISelectionInfo selectionInfo)
                        {
                            if (selectionInfo.IsSelected(e.NewStartingIndex))
                            {
                                SelectedItem = list[e.NewStartingIndex];
                                System.Diagnostics.Debug.WriteLine("SelectedItem {0}", SelectedItem);
                            }
                        }
                    }
                }
            }
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsMultipleSelection)
            {
                if (gridview.SelectedItems != null)
                {
                    SelectedItemsCount = gridview.SelectedItems.Count;
                }
                else if (gridview.SelectedRanges != null)
                {
                    var ranges = gridview.SelectedRanges;
                    SelectedItemsCount = ranges.IndexCount();
                    SelectRangesCommand?.TryExecute(ranges.GetIndexRanges().ToArray());
                }

                if (e.AddedItems != null)
                {
                    SelectItemsCommand?.TryExecute(e.AddedItems);
                }
                if (e.RemovedItems != null)
                {
                    DeselectItemsCommand?.TryExecute(e.RemovedItems);
                }
            }
        }

        private void OnItemClick(object sender, ItemClickEventArgs e)
        {
            if (!IsMultipleSelection)
            {
                ItemInvokedCommand?.TryExecute(e.ClickedItem);
            }
        }
        private void OnDoubleTapped(object sender, Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            if (!IsMultipleSelection)
            {
                ItemSecondaryActionInvokedCommand?.TryExecute(gridview.SelectedItem);
            }
        }

        private void OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            QuerySubmittedCommand?.TryExecute(args.QueryText);
        }

        private void OnToolbarClick(object sender, ToolbarButtonClickEventArgs e)
        {
            switch (e.ClickedButton)
            {
                case ToolbarButton.New:
                    NewCommand?.TryExecute();
                    break;
                case ToolbarButton.Delete:
                    DeleteCommand?.TryExecute();
                    break;
                case ToolbarButton.Select:
                    StartSelectionCommand?.TryExecute();
                    break;
                case ToolbarButton.Refresh:
                    RefreshCommand?.TryExecute();
                    break;
                case ToolbarButton.Cancel:
                    CancelSelectionCommand?.TryExecute();
                    break;
            }
        }

        #region NotifyPropertyChanged
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion


    }
}
