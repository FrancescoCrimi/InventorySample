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
        //[DatabaseGenerat‌​ed(DatabaseGeneratedOption.None)]
        public virtual TId Id
        {
            get => _id;
            //protected set => id = value;
            set => _id = value;
        }
    }
}
