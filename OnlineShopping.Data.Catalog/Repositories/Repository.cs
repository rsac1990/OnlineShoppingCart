namespace OnlineShopping.Data.Catalog.Repositories;

using Microsoft.EntityFrameworkCore;
using OnlineShopping.Domain.Catalog.Entities;
using OnlineShopping.Domain.Catalog.Interfaces;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public async Task<List<TEntity>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<TEntity?> GetAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task AddAsync(TEntity newEntity)
    {
        await _dbSet.AddAsync(newEntity);
    }

    public void Remove(TEntity entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
