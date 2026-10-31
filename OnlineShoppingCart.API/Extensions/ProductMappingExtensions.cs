namespace OnlineShopping.Api.Extensions;

using Domain.Catalog.Entities;
using Dtos;

public static class ProductMappingExtensions
{
    public static ProductDto ToDto(this Product product)
    {
        if (product == null) return null!;

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Image = product.Image,
            CategoryId = product.CategoryId,
            CategoryName = product.Category.Name,
            Price = product.Price,
            Amount = product.Amount,
        };
    }

    public static IEnumerable<ProductDto> ToDtoList(this IEnumerable<Product> categories) =>
        categories.Select(c => c.ToDto());

}
