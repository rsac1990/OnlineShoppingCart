namespace OnlineShopping.Api.Interfaces;

using Dtos;

public interface IProductService
{
    Task<List<ProductDto>> GetAllAsync();
    Task<OperationResult> AddAsync(ProductDto newProduct);
    Task<OperationResult> UpdateAsync(int id, ProductDto product);
    Task<OperationResult> DeleteAsync(int id);
}