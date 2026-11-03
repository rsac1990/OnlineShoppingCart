namespace OnlineShopping.Domain.Catalog.Interfaces;

using Domain.Catalog.Entities;

public interface IProductRepository : IRepository<Product>
{
    Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedListAsync(int pageNumber,
                                                                         int pageSize,
                                                                         int? categoryId);
}
