using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Domain.Common
{
    public abstract class ObservableObject<T> : CommunityToolkit.Mvvm.ComponentModel.ObservableObject where T : ObservableObject<T>
    {
        [Key]
        [DatabaseGenerat‌​ed(DatabaseGeneratedOption.None)]
        public long Id
        {
            get; set;
        }

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
