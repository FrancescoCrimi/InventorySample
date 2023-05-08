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

namespace Inventory.Uwp.Dto
{
    public class CountryDto : ObservableDto<CountryDto>
    {
        private string _code;
        private string _name;

        public string Code
        {
            get => _code;
            set => SetProperty(ref _code, value);
        }
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public override void Merge(CountryDto source)
            => throw new NotImplementedException();
    }
}
