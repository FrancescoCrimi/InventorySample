// Copyright (c) Microsoft. All rights reserved.
// Copyright (c) 2023 Francesco Crimi francrim@gmail.com
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.

using Inventory.Uwp.Dto;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Inventory.Uwp.Views.Orders
{
    public sealed partial class OrdersCard : UserControl
    {
        public OrdersCard()
        {
            InitializeComponent();
        }

        #region Item
        public OrderDto Item
        {
            get { return (OrderDto)GetValue(ItemProperty); }
            set { SetValue(ItemProperty, value); }
        }

        public static readonly DependencyProperty ItemProperty =
            DependencyProperty.Register(nameof(Item),
                                        typeof(OrderDto),
                                        typeof(OrdersCard),
                                        new PropertyMetadata(null));
        #endregion
    }
}
