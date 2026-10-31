namespace OnlineShopping.Api.Services;

using System.Threading.Tasks;
using Domain.Catalog;
using Domain.Catalog.Entities;
using Domain.Catalog.Interfaces;
using Interfaces;

public class CategoryService : ICategoryService
{
    private IRepository<Category> _categoryRepository;

    public CategoryService(IRepository<Category> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<List<Category>> GetAllAsync() =>
        await _categoryRepository.GetAllAsync() ?? new List<Category>();

    public async Task<OperationResult> AddAsync(Category category)
    {
        try
        {
            var newCategory = new Category(category.Name);
            newCategory.SetParentCategory(category.ParentCategoryId);
            newCategory.SetImage(category.Image);

            await _categoryRepository.AddAsync(newCategory);
            await _categoryRepository.SaveChangesAsync();

            return OperationResult.Success();
        }
        catch (DomainException ex)
        {
            return OperationResult.Failure(ex.Message, ErrorType.Validation);
        }

    }

    public async Task<OperationResult> UpdateAsync(int id, Category cat)
    {
        var existingCategory = await _categoryRepository.GetAsync(id);
        if (existingCategory == null)
        {
            return OperationResult.NotFound($"Categoría con ID {id} no encontrada.");
        }

        try
        {
            existingCategory.Update(cat.Name);
            existingCategory.SetParentCategory(cat.ParentCategoryId);
            existingCategory.SetImage(cat.Image);

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
