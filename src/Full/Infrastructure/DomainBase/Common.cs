namespace Inventory.Infrastructure.DomainBase
{
    public interface IEntity
    {
        long Id
        {
            get; set;
        }
    }


    public class Entity : GenericEntity<long>, IEntity { }

    public class ValueObject : GenericValueObject<long> { }

    public interface IRepository<TEntity> : IGenericRepository<TEntity, long>
        where TEntity : Entity
    { }
}
