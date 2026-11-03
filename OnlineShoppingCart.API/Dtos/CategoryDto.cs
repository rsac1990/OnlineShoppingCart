namespace OnlineShopping.Api.Dtos;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Image { get; set; }
    public int? ParentCategoryId { get; set; }
    public string? ParentCategoryName { get; set; }

    public List<LinkDto> Links { get; set; } = new();
}

public class LinkDto
{
    public string Href { get; set; }
    public string Rel { get; set; }
    public string Method { get; set; }
}