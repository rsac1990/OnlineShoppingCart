namespace OnlineShopping.Domain.Catalog.Services;

using Entities;
using Interfaces;

public class ProductService
{
    private IRepository<Product> _productRepository;

    public ProductService(IRepository<Product> productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<List<Product>> GetAllAsync()=>
        await _productRepository.GetAllAsync() ?? new List<Product>();

    public async Task<Product> GetAsync(int id) =>
        await _productRepository.GetAsync(id);

    public async Task AddAsync(Product newProduct) =>
        await _productRepository.AddAsync(newProduct);

    public async Task UpdateAsync(int id, Product product) =>
        await _productRepository.UpdateAsync(id, product);

    public async Task DeleteAsync(int id) =>
        await _productRepository.RemoveAsync(id);
}
