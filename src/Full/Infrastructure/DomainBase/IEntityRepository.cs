using System.Threading.Tasks;

namespace Inventory.Infrastructure.DomainBase
{
    public interface IEntityRepository<TEntity, TId> where TEntity : Entity<TId>
    {
        Task<TEntity> GetById(TId id);
        Task<TId> Save(TEntity entity);
        Task Update(TEntity entity);
        Task Delete(TId id);
    }
}
