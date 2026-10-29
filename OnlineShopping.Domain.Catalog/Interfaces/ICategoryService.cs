namespace OnlineShopping.Domain.Catalog.Interfaces;

using OnlineShopping.Domain.Catalog.Entities;

public interface ICategoryService
{
    Task AddAsync(Category newCategory);
    Task DeleteAsync(int id);
    Task<List<Category>> GetAllAsync();
    Task<Category?> GetAsync(int categoryId);
    Task UpdateAsync(int id, Category category);
}