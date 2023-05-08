using System.Threading.Tasks;

namespace Inventory.Infrastructure.DomainBase
{
    public interface IRepository<TEntity> : IGenericRepository<TEntity, long>
        where TEntity : Entity<TEntity>
    { }

    public interface IGenericRepository<TEntity, TId> where TEntity : Entity<TEntity>
    {
        Task<TEntity> GetById(TId id);
        Task<TId> Save(TEntity entity);
        Task Update(TEntity entity);
        Task Delete(TId id);
    }
}
