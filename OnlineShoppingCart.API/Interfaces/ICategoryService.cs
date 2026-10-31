namespace OnlineShopping.Api.Interfaces;

using Domain.Catalog.Entities;
using Dtos;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetAllAsync();
    Task<OperationResult> AddAsync(CategoryDto newCategory);
    Task<OperationResult> UpdateAsync(int id, CategoryDto category);
    Task<OperationResult> DeleteAsync(int id);
}