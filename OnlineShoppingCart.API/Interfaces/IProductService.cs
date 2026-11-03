namespace OnlineShopping.Api.Interfaces;

using Dtos;

public interface IProductService
{
    Task<PagedResult<ProductDto>> GetListAsync(ProductParametersDto parameters);
    Task<OperationResult> AddAsync(ProductDto newProduct);
    Task<OperationResult> UpdateAsync(int id, ProductDto product);
    Task<OperationResult> DeleteAsync(int id);
}