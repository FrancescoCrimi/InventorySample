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
    public class ShipperDto : ObservableDto<ShipperDto>
    {
        private string _name;
        private string _phone;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        public string Phone
        {
            get => _phone;
            set => SetProperty(ref _phone, value);
        }

        public override void Merge(ShipperDto source)
            => throw new System.NotImplementedException();
    }
}
