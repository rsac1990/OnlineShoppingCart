namespace OnlineShopping.Api.Extensions;

using Domain.Catalog.Entities;
using Dtos;

public static class CategoryMappingExtensions
{
    public static CategoryDto ToDto(this Category category)
    {
        if (category == null) return null!;

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Image = category.Image,
            ParentCategoryId = category.ParentCategoryId,
            ParentCategoryName = category.ParentCategory?.Name,
            Links = new List<LinkDto>()
        };
    }

    public static IEnumerable<CategoryDto> ToDtoList(this IEnumerable<Category> categories) =>
        categories.Select(c => c.ToDto());

}
