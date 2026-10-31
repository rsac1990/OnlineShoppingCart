namespace OnlineShopping.Domain.Catalog.Entities;

using Ganss.Xss;
using Helpers;

public class Product : Entity
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public string? Image { get; private set; }
    public int CategoryId { get; private set; }
    public Category Category { get; private set; }
    public decimal Price { get; private set; }
    public int Amount { get; private set; }

    public Product()
    {
        
    }

    public Product(string name, int categoryId, decimal price, int amount)
    {
        ValidateProductInfo(name, categoryId, price, amount);

        Name = name;
        CategoryId = categoryId;
        Price = price;
        Amount = amount;
    }

    public void SetDescription(string? description)
    {
        if (string.IsNullOrEmpty( description))
        {
            return;
        }

        var sanitizer = new HtmlSanitizer();
        Description = sanitizer.Sanitize(description);
    }

    public void SetImage(string? image)
    {
        if (string.IsNullOrEmpty(image))
        {
            return;
        }

        if (!EntityHelper.IsValidImageUrl(image))
        {
            throw new DomainException("La URL de la imagen no es válida.");
        }

        Image = image;
    }

    public void Update(string name, int categoryId, decimal price, int amount)
    {
        ValidateProductInfo(name, categoryId, price, amount);

        Name = name;
        CategoryId = categoryId;
        Price = price;
        Amount = amount;
    }

    private static void ValidateProductInfo(string name, int categoryId, decimal price, int amount)
    {
        if (string.IsNullOrEmpty(name) || name.Length > 50)
        {
            throw new DomainException("Nombre inválido, debe tener máximo 50 caracteres");
        }

        if (price < 0)
        {
            throw new DomainException("El precio no puede ser negativo");
        }

        if (amount < 0) // (Note: depending on business logic, maybe amount can be 0?)
        {
            throw new DomainException("La cantidad no puede ser negativa");
        }
    }
}
