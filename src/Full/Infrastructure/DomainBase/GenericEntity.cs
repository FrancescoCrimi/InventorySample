using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Inventory.Infrastructure.DomainBase
{
    [Serializable]
    [DataContract(Name = "EntityOfint", Namespace = "http://gest.cicciosoft.tk")]
    public abstract class GenericEntity<TId>
    {
        private TId id;

        [Key]
        //[DatabaseGenerat‌​ed(DatabaseGeneratedOption.None)]
        [DataMember]
        public virtual TId Id
        {
            get => id;
            //protected set => id = value;
            set => id = value;
        }

        public override string ToString()
        {
            return this.GetType().Name + " [Id=" + Id.ToString() + "]";
        }
    }
}
