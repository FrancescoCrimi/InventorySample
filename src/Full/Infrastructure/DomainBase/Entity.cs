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

using System;
using System.ComponentModel.DataAnnotations;

namespace Inventory.Infrastructure.DomainBase
{
    public class Entity<TEntity> : GenericEntity<long>, IEquatable<TEntity>
        where TEntity : Entity<TEntity>
    {
        public override string ToString()
            => GetType().Name + " [Id=" + Id.ToString() + "]";

        public override bool Equals(object obj)
            => Equals(obj as TEntity);

        public bool Equals(TEntity other)
            => !(other is null) && Id == other.Id;

        public override int GetHashCode()
            => HashCode.Combine(base.GetHashCode(), Id);
    }

    public class GenericEntity<TId>
    {
        private TId _id;

        [Key]
        public virtual TId Id
        {
            get => _id;
            //protected set => id = value;
            set => _id = value;
        }
    }
}
