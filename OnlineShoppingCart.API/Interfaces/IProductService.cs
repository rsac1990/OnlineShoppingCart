namespace OnlineShopping.Api.Interfaces;

using OnlineShopping.Domain.Catalog.Entities;

public interface IProductService
{
    Task AddAsync(Product newProduct);
    Task DeleteAsync(int id);
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetAsync(int id);
    Task UpdateAsync(int id, Product product);
}