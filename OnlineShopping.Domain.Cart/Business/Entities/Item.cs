namespace OnlineShopping.Domain.Cart.Business.Entities;

public class Item : Entity
{
    public string Name { get; set; }
    public ItemImage? Image { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }

    public Item(int id, string name, ItemImage? image, decimal price, int quantity)
    {
        if (id <= 0)
            throw new ArgumentException("El id del item es obligatorio", nameof(id));

        if (price < 0)
            throw new ArgumentException("El precio no puede ser negativo", nameof(price));

        if (quantity < 0)
            throw new ArgumentException("La cantidad no puede ser negativa", nameof(quantity));

        Id = id;
        Name = name;
        Image = image;
        Price = price;
        Quantity = quantity;
    }
}

public record ItemImage(string Url, string AltText);