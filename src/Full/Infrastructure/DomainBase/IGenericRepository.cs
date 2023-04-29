using System.Threading.Tasks;

namespace Inventory.Infrastructure.DomainBase
{
    public interface IGenericRepository<TEntity, TId> where TEntity : GenericEntity<TId>
    {
        Task<TEntity> GetById(TId id);
        Task<TId> Save(TEntity entity);
        Task Update(TEntity entity);
        Task Delete(TId id);
    }
}
