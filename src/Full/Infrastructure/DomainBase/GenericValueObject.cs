using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Inventory.Infrastructure.DomainBase
{
    [Serializable]
    [DataContract(Name = "ValueObjectOfint", Namespace = "http://gest.cicciosoft.tk")]
    public abstract class GenericValueObject<TId> : INotifyPropertyChanged
    {
        private TId id;
        [DataMember]
        public virtual TId Id
        {
            get => id; protected set => id = value;
        }

        public virtual event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
