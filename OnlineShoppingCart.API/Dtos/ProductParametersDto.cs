namespace OnlineShopping.Api.Dtos;

public class ProductParametersDto
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int? CategoryId { get; set; } // Optional filter by Category
}
