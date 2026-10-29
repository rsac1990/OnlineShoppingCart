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

    public async Task AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, TEntity entity)
    {
        var existingEntity = await _dbSet.FindAsync(id);

        if (existingEntity is null)
        {
            throw new ArgumentException($"Entidad con el {id} no encontrada.");
        }

        entity.Id = id;
        _context.Entry(existingEntity).CurrentValues.SetValues(entity);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(int id)
    {
        var entity = await GetAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
