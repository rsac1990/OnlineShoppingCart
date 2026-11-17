namespace OnlineShopping.Api.Services;

using System.Text.Json;
using Data.Catalog;
using Data.Catalog.Outbox;
using Domain.Catalog;
using Domain.Catalog.Entities;
using Domain.Catalog.Interfaces;
using Dtos;
using Extensions;
using Interfaces;

public class ProductService : IProductService
{
    private IProductRepository productRepository;
    private readonly AppDbContext dbContext;

    public ProductService(IProductRepository productRepository, AppDbContext dbContext)
    {
        this.productRepository = productRepository;
        this.dbContext = dbContext;
    }

    public async Task<PagedResult<ProductDto>> GetListAsync(ProductParametersDto parameters)
    {
        var (items, totalCount) = await productRepository.GetPagedListAsync(
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

            await productRepository.AddAsync(newProduct);
            await productRepository.SaveChangesAsync();

            return OperationResult.Success();
        }
        catch (DomainException ex)
        {
            return OperationResult.Failure(ex.Message, ErrorType.Validation);
        }

    }

    public async Task<OperationResult> UpdateAsync(int id, ProductDto productDto)
    {
        var existingProduct = await dbContext.Products.FindAsync(id);
        if (existingProduct == null)
        {
            return OperationResult.NotFound($"Producto con ID {id} no encontrado.");
        }

        try
        {
            UpdateExistingProduct(productDto, existingProduct);

            var outboxMessage = GetOutboxMessage(existingProduct);

            await dbContext.OutboxMessages.AddAsync(outboxMessage);
            await dbContext.SaveChangesAsync();

            return OperationResult.Success();
        }
        catch (DomainException ex)
        {
            return OperationResult.Failure(ex.Message, ErrorType.Validation);
        }
    }

    public async Task<OperationResult> DeleteAsync(int id)
    {
        var existingProduct = await productRepository.GetAsync(id);

        if (existingProduct is null)
        {
            return OperationResult.NotFound($"Producto con ID {id} no encontrado.");
        }

        productRepository.Remove(existingProduct);
        await productRepository.SaveChangesAsync();

        return OperationResult.Success();
    }

    private static void UpdateExistingProduct(ProductDto productDto, Product existingProduct)
    {
        existingProduct.Update(productDto.Name,
                                           productDto.CategoryId,
                                           productDto.Price,
                                           productDto.Amount);

        existingProduct.SetDescription(productDto.Description);
        existingProduct.SetImage(productDto.Image);
    }

    private OutboxMessage GetOutboxMessage(Product existingProduct)
    {
        var updatedEvent = new ProductUpdatedEvent
        {
            Id = existingProduct.Id,
            Name = existingProduct.Name,
            Price = existingProduct.Price,
            ImageUrl = existingProduct.Image
        };
        return new OutboxMessage
        {
            Type = nameof(ProductUpdatedEvent),
            Payload = JsonSerializer.Serialize(updatedEvent),
            OccurredOnUtc = DateTime.UtcNow
        };
    }

}
