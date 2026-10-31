namespace OnlineShopping.Api.Services;

using Domain.Catalog;
using Domain.Catalog.Entities;
using Domain.Catalog.Interfaces;
using Interfaces;

public class ProductService : IProductService
{
    private IRepository<Product> _productRepository;

    public ProductService(IRepository<Product> productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<List<Product>> GetAllAsync() =>
       await _productRepository.GetAllAsync() ?? new List<Product>();

    public async Task<OperationResult> AddAsync(Product product)
    {
        try
        {
            var newProduct = new Product(product.Name, product.CategoryId, product.Price, product.Amount);
            newProduct.SetImage(product.Image);
            newProduct.SetDescription(product.Description);

            await _productRepository.AddAsync(newProduct);
            await _productRepository.SaveChangesAsync();

            return OperationResult.Success();
        }
        catch (DomainException ex)
        {
            return OperationResult.Failure(ex.Message, ErrorType.Validation);
        }

    }

    public async Task<OperationResult> UpdateAsync(int id, Product prod)
    {
        var existingProduct = await _productRepository.GetAsync(id);
        if (existingProduct == null)
        {
            return OperationResult.NotFound($"Producto con ID {id} no encontrado.");
        }

        try
        {
            existingProduct.Update(prod.Name, prod.CategoryId, prod.Price, prod.Amount);
            existingProduct.SetDescription(prod.Description);
            existingProduct.SetImage(prod.Image);

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
