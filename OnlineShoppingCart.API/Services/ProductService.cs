namespace OnlineShopping.Api.Services;

using Domain.Catalog;
using Domain.Catalog.Entities;
using Domain.Catalog.Interfaces;
using Dtos;
using Interfaces;
using Extensions;

public class ProductService : IProductService
{
    private IRepository<Product> _productRepository;

    public ProductService(IRepository<Product> productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<List<ProductDto>> GetAllAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return products.ToDtoList().ToList() ?? new List<ProductDto>();
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
