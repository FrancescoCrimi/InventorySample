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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.DependencyInjection;
using Inventory.Application;
using Inventory.Domain.Aggregates.CustomerAggregate;
using Inventory.Infrastructure.Common;
using Inventory.Uwp.Library.Controls;
using Inventory.Uwp.Library.Extensions;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Inventory.Uwp.Controls
{
    public sealed partial class CustomerSuggestBox : UserControl
    {
        public CustomerSuggestBox()
        {
            if (!DesignMode.DesignModeEnabled)
            {
                CustomerService = Ioc.Default.GetService<CustomerService>();
            }
            InitializeComponent();
        }

        private CustomerService CustomerService
        {
            get;
        }

        #region Items
        public IList<Customer> Items
        {
            get => (IList<Customer>)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register(nameof(Items),
                                        typeof(IList<Customer>),
                                        typeof(CustomerSuggestBox),
                                        new PropertyMetadata(null));
        #endregion

        #region DisplayText
        public string DisplayText
        {
            get => (string)GetValue(DisplayTextProperty);
            set => SetValue(DisplayTextProperty, value);
        }

        public static readonly DependencyProperty DisplayTextProperty =
            DependencyProperty.Register(nameof(DisplayText),
                                        typeof(string),
                                        typeof(CustomerSuggestBox),
                                        new PropertyMetadata(null));
        #endregion

        #region IsReadOnly*
        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        private static void IsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as CustomerSuggestBox;
            control.suggestBox.Mode = ((bool)e.NewValue == true) ? FormEditMode.ReadOnly : FormEditMode.Auto;
        }

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly),
                                        typeof(bool),
                                        typeof(CustomerSuggestBox),
                                        new PropertyMetadata(false, IsReadOnlyChanged));
        #endregion

        #region CustomerSelectedCommand
        public ICommand CustomerSelectedCommand
        {
            get => (ICommand)GetValue(CustomerSelectedCommandProperty);
            set => SetValue(CustomerSelectedCommandProperty, value);
        }

        public static readonly DependencyProperty CustomerSelectedCommandProperty =
            DependencyProperty.Register(nameof(CustomerSelectedCommand),
                                        typeof(ICommand),
                                        typeof(CustomerSuggestBox),
                                        new PropertyMetadata(null));
        #endregion

        private async void OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                if (args.CheckCurrent())
                {
                    Items = String.IsNullOrEmpty(sender.Text) ? null : await GetItems(sender.Text);
                }
            }
        }

        private async Task<IList<Customer>> GetItems(string query)
        {
            var request = new DataRequest<Customer>()
            {
                Query = query,
                OrderBy = r => r.FirstName
            };
            return await CustomerService.GetCustomersAsync(0, 20, request);
        }

        private void OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            CustomerSelectedCommand?.TryExecute(args.SelectedItem);
        }
    }
}
