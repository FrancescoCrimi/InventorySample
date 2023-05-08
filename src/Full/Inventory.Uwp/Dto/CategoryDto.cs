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
    public class CategoryDto : ObservableDto<CategoryDto>
    {
        private string _name;
        private string _description;
        private byte[] _picture;
        private byte[] _thumbnail;


        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }
        public byte[] Picture
        {
            get => _picture;
            set => SetProperty(ref _picture, value);
        }
        public byte[] Thumbnail
        {
            get => _thumbnail;
            set => SetProperty(ref _thumbnail, value);
        }

        public override void Merge(CategoryDto source)
            => throw new NotImplementedException();
    }
}
