namespace OnlineShopping.Domain.Catalog.Services;

using Entities;
using Interfaces;

public class CategoryService
{
    private IRepository<Category> _categoryRepository;

    public CategoryService(IRepository<Category> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<List<Category>> GetAllAsync()=>
        await _categoryRepository.GetAllAsync() ?? new List<Category>();

    public async Task<Category> GetAsync(int categoryId) =>
        await _categoryRepository.GetAsync(categoryId);

    public async Task AddAsync(Category newCategory) =>
        await _categoryRepository.AddAsync(newCategory);

    public async Task UpdateAsync(int id, Category category) =>
        await _categoryRepository.UpdateAsync(id, category);

    public async Task DeleteAsync(int id) =>
        await _categoryRepository.RemoveAsync(id);
}
