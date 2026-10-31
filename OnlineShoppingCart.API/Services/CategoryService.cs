namespace OnlineShopping.Api.Services;

using Domain.Catalog;
using Domain.Catalog.Entities;
using Domain.Catalog.Interfaces;
using Interfaces;
using Dtos;
using OnlineShopping.Api.Extensions;

public class CategoryService : ICategoryService
{
    private IRepository<Category> _categoryRepository;

    public CategoryService(IRepository<Category> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<List<CategoryDto>> GetAllAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return categories.ToDtoList().ToList() ?? new List<CategoryDto>();
    }

    public async Task<OperationResult> AddAsync(CategoryDto categoryDto)
    {
        try
        {
            var newCategory = new Category(categoryDto.Name);
            newCategory.SetParentCategory(categoryDto.ParentCategoryId);
            newCategory.SetImage(categoryDto.Image);

            await _categoryRepository.AddAsync(newCategory);
            await _categoryRepository.SaveChangesAsync();

            return OperationResult.Success();
        }
        catch (DomainException ex)
        {
            return OperationResult.Failure(ex.Message, ErrorType.Validation);
        }

    }

    public async Task<OperationResult> UpdateAsync(int id, CategoryDto categoryDto)
    {
        var existingCategory = await _categoryRepository.GetAsync(id);
        if (existingCategory == null)
        {
            return OperationResult.NotFound($"Categoría con ID {id} no encontrada.");
        }

        try
        {
            existingCategory.Update(categoryDto.Name);
            existingCategory.SetParentCategory(categoryDto.ParentCategoryId);
            existingCategory.SetImage(categoryDto.Image);

            await _categoryRepository.SaveChangesAsync();

            return OperationResult.Success();
        }
        catch (DomainException ex)
        {
            return OperationResult.Failure(ex.Message, ErrorType.Validation);
        }
    }

    public async Task<OperationResult> DeleteAsync(int id)
    {
        var existingCategory = await _categoryRepository.GetAsync(id);
        
        if (existingCategory is null)
        {
            return OperationResult.NotFound($"Categoría con ID {id} no encontrada.");
        }

        _categoryRepository.Remove(existingCategory);
        await _categoryRepository.SaveChangesAsync();

        return OperationResult.Success();
    }

}
