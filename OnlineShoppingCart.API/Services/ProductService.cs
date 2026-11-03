namespace OnlineShopping.Api.Services;

using Domain.Catalog;
using Domain.Catalog.Entities;
using Domain.Catalog.Interfaces;
using Dtos;
using Interfaces;
using Extensions;

public class ProductService : IProductService
{
    private IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<PagedResult<ProductDto>> GetListAsync(ProductParametersDto parameters)
    {        
        var (items, totalCount) = await _productRepository.GetPagedListAsync(
            parameters.PageNumber,
            parameters.PageSize,
            parameters.CategoryId
        );

        var productDtos = items.ToDtoList().ToList() ?? new List<ProductDto>();

        return new PagedResult<ProductDto>
        {
            Items = productDtos,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<OperationResult> AddAsync(ProductDto productDto)
    {
        try
        {
            var newProduct = new Product(productDto.Name,
                                            productDto.CategoryId,
                                            productDto.Price,
                                            productDto.Amount);

            newProduct.SetImage(productDto.Image);
            newProduct.SetDescription(productDto.Description);

            await _productRepository.AddAsync(newProduct);
            await _productRepository.SaveChangesAsync();

            return OperationResult.Success();
        }
        catch (DomainException ex)
        {
            return OperationResult.Failure(ex.Message, ErrorType.Validation);
        }

    }

    public async Task<OperationResult> UpdateAsync(int id, ProductDto productDto)
    {
        var existingProduct = await _productRepository.GetAsync(id);
        if (existingProduct == null)
        {
            return OperationResult.NotFound($"Producto con ID {id} no encontrado.");
        }

        try
        {
            existingProduct.Update(productDto.Name,
                                   productDto.CategoryId,
                                   productDto.Price,
                                   productDto.Amount);

            existingProduct.SetDescription(productDto.Description);
            existingProduct.SetImage(productDto.Image);

            await _productRepository.SaveChangesAsync();

            return OperationResult.Success();
        }
        catch (DomainException ex)
        {
            return OperationResult.Failure(ex.Message, ErrorType.Validation);
        }
    }

    public async Task<OperationResult> DeleteAsync(int id)
    {
        var existingProduct = await _productRepository.GetAsync(id);

        if (existingProduct is null)
        {
            return OperationResult.NotFound($"Producto con ID {id} no encontrado.");
        }

        _productRepository.Remove(existingProduct);
        await _productRepository.SaveChangesAsync();

        return OperationResult.Success();
    }
}
