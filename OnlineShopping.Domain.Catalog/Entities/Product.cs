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

    public Product(string name, Category category, decimal price, int amount)
    {
        if (string.IsNullOrEmpty(name) || name.Length > 50)
        {
            throw new DomainException("Nombre inválido, debe tener máximo 50 caracteres");
        }

        if (category is null)
        {
            throw new DomainException("La categoria es obligatoria");
        }

        if (price < 0)
        {
            throw new DomainException("El precio no puede ser negativo");
        }

        if (amount <= 0)
        {
            throw new DomainException("La cantidad no puede ser negativa");
        }

        Name = name;
        Category = category;
        CategoryId = category.Id;
        Price = price;
        Amount = amount;
    }

    public void SetDescription(string description)
    {
        var sanitizer = new HtmlSanitizer();
        Description = sanitizer.Sanitize(description);
    }

    public void SetImage(string image)
    {
        if (!EntityHelper.IsValidImageUrl(image))
        {
            throw new DomainException("La URL de la imagen no es válida.");
        }

        Image = image;
    }
}
