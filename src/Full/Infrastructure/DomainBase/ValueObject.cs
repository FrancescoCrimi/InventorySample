namespace Inventory.Infrastructure.DomainBase
{
    public class ValueObject : GenericValueObject<long> { }

    public abstract class GenericValueObject<TId>
    {
        private TId id;
        public virtual TId Id
        {
            get => id;
            //protected set => id = value;
            set => id = value;
        }
    }
}
