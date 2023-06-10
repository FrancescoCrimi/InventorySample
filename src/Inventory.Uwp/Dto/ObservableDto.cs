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
    public abstract class ObservableDto<TEntity> : GenericObservableDto<long>, IEquatable<TEntity>
        where TEntity : ObservableDto<TEntity>
    {
        public void NotifyChanges()
        {
            // Notify all properties
            OnPropertyChanged("");
        }

        public abstract void Merge(TEntity source);

        public bool IsEmpty { get; set; }

        public bool IsNew => Id <= 0;

        public override bool Equals(object obj)
            => Equals(obj as TEntity);

        public bool Equals(TEntity other)
            => !(other is null) && Id == other.Id;

        public override int GetHashCode()
            => HashCode.Combine(Id);
    }

    public abstract class GenericObservableDto<TId> : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
    {
        private TId _id;
        public TId Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }
    }
}
