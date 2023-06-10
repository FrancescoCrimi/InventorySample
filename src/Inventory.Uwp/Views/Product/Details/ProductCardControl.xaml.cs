﻿// Copyright (c) Microsoft. All rights reserved.
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
using Inventory.Uwp.ViewModels.Products;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Inventory.Uwp.Views.Products
{
    public sealed partial class ProductCardControl : UserControl
    {
        public ProductCardControl()
        {
            InitializeComponent();
        }

        #region ViewModel
        public ProductDetailsViewModel ViewModel
        {
            get { return (ProductDetailsViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel),
                                        typeof(ProductDetailsViewModel),
                                        typeof(ProductCardControl),
                                        new PropertyMetadata(null));
        #endregion

        #region Item
        public ProductDto Item
        {
            get { return (ProductDto)GetValue(ItemProperty); }
            set { SetValue(ItemProperty, value); }
        }

        public static readonly DependencyProperty ItemProperty =
            DependencyProperty.Register(nameof(Item),
                                        typeof(ProductDto),
                                        typeof(ProductCardControl),
                                        new PropertyMetadata(null));
        #endregion
    }
}
