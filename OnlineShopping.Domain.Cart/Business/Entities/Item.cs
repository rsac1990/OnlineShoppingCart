namespace OnlineShopping.Domain.Cart.Business.Entities;

public class Item
{
    public int Id { get; protected set; }
    public string Name { get; private set; }
    public ItemImage? Image { get; private set; }
    public decimal Price { get; private set; }
    public int Quantity { get; private set; }

    public Item(int id, string name, ItemImage? image, decimal price, int quantity)
    {
        if (id <= 0)
            throw new DomainException("El id del item es obligatorio");

        if (quantity < 0)
            throw new DomainException("La cantidad no puede ser negativa");

        ValidateInfo(name, price);

        Id = id;
        Name = name;
        Image = image;
        Price = price;
        Quantity = quantity;
    }

    public void Update(string name, ItemImage? image, decimal price)
    {
        ValidateInfo(name, price);

        Name = name;
        Image = image;
        Price = price;
    }

    public void UpdateQuantity(int quantity)
    {
        Quantity += quantity;

        if (Quantity <= 0)
        {
            throw new DomainException("La cantidad no puede ser igual o menor a 0");
        }
    }

    private static void ValidateInfo(string name, decimal price)
    {
        if (string.IsNullOrEmpty(name))
            throw new DomainException("El nombre del item es obligatorio");

        if (price < 0)
            throw new DomainException("El precio no puede ser negativo");
    }
}

public record ItemImage(string Url, string AltText);