namespace OnlineShopping.Data.Catalog.Repositories;

using Microsoft.EntityFrameworkCore;
using OnlineShopping.Domain.Catalog.Entities;
using OnlineShopping.Domain.Catalog.Interfaces;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext context) : base(context) { }

    public async Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedListAsync(int pageNumber, 
                                                                                      int pageSize, 
                                                                                      int? categoryId)
    {
        var query = _dbSet.AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value).Include(p => p.Category);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}
