namespace OnlineShopping.Api.Interfaces;

using OnlineShopping.Domain.Catalog.Entities;

public interface IProductService
{
    Task<List<Product>> GetAllAsync();
    Task<OperationResult> AddAsync(Product newProduct);
    Task<OperationResult> UpdateAsync(int id, Product product);
    Task<OperationResult> DeleteAsync(int id);
}