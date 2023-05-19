// Copyright (c) 2023 Francesco Crimi francrim@gmail.com
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Infrastructure.Common
{
    public abstract class Entity : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
    {
        private long _id;

        [Key]
        public long Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }
    }

    public abstract class ObservableObject<T> : Entity where T : ObservableObject<T>
    {
        [NotMapped]
        public bool IsNew => Id <= 0;

        [NotMapped]
        public bool IsEmpty
        {
            get; set;
        }
    }
}
