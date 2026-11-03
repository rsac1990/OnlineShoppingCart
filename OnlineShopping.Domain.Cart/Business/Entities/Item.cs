namespace OnlineShopping.Domain.Cart.Business.Entities;

public class Item : Entity
{
    public string Name { get; set; }
    public ItemImage? Image { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }

    public Item(string id, string name, ItemImage? image, decimal price, int quantity)
    {
        if (string.IsNullOrEmpty(id))
             throw new DomainException("El id del item es obligatorio");

        if (price < 0)
             throw new DomainException("El precio no puede ser negativo");

        if (quantity < 0)
             throw new DomainException("La cantidad no puede ser negativa");

        Id = id;
        Name = name;
        Image = image;
        Price = price;
        Quantity = quantity;
    }
}

public record ItemImage(string Url, string AltText);