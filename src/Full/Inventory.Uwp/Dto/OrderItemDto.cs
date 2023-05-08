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


namespace Inventory.Uwp.Dto
{
    public class OrderItemDto : ObservableDto<OrderItemDto>
    {
        private int _quantity;
        private int _taxTypeId;
        private decimal _discount;
        private TaxTypeDto _taxTypeDto;

        public int OrderLine { get; set; }
        public int Quantity
        {
            get => _quantity;
            set { if (SetProperty(ref _quantity, value)) UpdateTotals(); }
        }
        public decimal UnitPrice { get; set; }
        public decimal Discount
        {
            get => _discount;
            set { if (SetProperty(ref _discount, value)) UpdateTotals(); }
        }

        public long OrderId { get; set; }
        public long ProductId { get; set; }
        public int TaxTypeId
        {
            get => _taxTypeId;
            set { if (SetProperty(ref _taxTypeId, value)) UpdateTotals(); }
        }

        public OrderDto Order { get; set; }
        public ProductDto Product { get; set; }
        public TaxTypeDto TaxType
        {
            get => _taxTypeDto;
            set { if (SetProperty(ref _taxTypeDto, value)) UpdateTotals(); }
        }


        public decimal Subtotal => Quantity * UnitPrice;
        public decimal Total => (decimal)((Subtotal - Discount) * (1 + TaxType?.Rate / 100m));

        private void UpdateTotals()
        {
            OnPropertyChanged(nameof(Subtotal));
            OnPropertyChanged(nameof(Total));
        }

        public override void Merge(OrderItemDto source)
        {
            if (source != null)
            {
                Id = source.Id;
                OrderLine = source.OrderLine;
                Quantity = source.Quantity;
                UnitPrice = source.UnitPrice;
                Discount = source.Discount;

                OrderId = source.OrderId;
                ProductId = source.ProductId;
                TaxTypeId = source.TaxTypeId;

                Order = source.Order;
                Product = source.Product;
                TaxType = source.TaxType;
            }
        }
    }
}
