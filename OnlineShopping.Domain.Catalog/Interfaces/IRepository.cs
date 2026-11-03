namespace OnlineShopping.Domain.Catalog.Interfaces;

using Entities;

public interface IRepository<TEntity> where TEntity : Entity
{
    Task<List<TEntity>> GetAllAsync();
    Task<TEntity?> GetAsync(int id);
    Task AddAsync(TEntity newEntity);
    void Remove(TEntity entity);
    Task<bool> SaveChangesAsync();
}
