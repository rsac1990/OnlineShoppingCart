namespace OnlineShopping.Api.Interfaces;

using Domain.Catalog.Entities;

public interface ICategoryService
{
    Task<List<Category>> GetAllAsync();
    Task<OperationResult> AddAsync(Category newCategory);
    Task<OperationResult> UpdateAsync(int id, Category category);
    Task<OperationResult> DeleteAsync(int id);
}