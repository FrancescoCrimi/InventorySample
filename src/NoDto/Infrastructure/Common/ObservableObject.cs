using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Infrastructure.Common
{
    public abstract class Entity : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
    {
        private long _id;

        [Key]
        [DatabaseGenerat‌​ed(DatabaseGeneratedOption.None)]
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

        public abstract void Merge(T source);

        // Notify all properties
        public void NotifyChanges()
        {
            OnPropertyChanged("");
        }
    }
}
