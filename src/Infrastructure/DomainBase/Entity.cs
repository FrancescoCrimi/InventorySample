using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Inventory.Infrastructure.DomainBase
{
    [Serializable]
    [DataContract(Name = "EntityOfint", Namespace = "http://gest.cicciosoft.tk")]
    public abstract class Entity<TId> : INotifyPropertyChanged
    {
        private TId id;

        [DataMember]
        public virtual TId Id
        {
            get
            {
                return id;
            }
            protected set
            {
                id = value;
            }
        }

        public virtual event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            return this.GetType().Name + " [Id=" + Id.ToString() + "]";
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
