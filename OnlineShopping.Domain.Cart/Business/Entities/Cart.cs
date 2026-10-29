namespace OnlineShopping.Domain.Cart.Business.Entities;

public class Cart : Entity
{
    private List<Item> _items = new();

    public IReadOnlyCollection<Item> Items => _items.AsReadOnly();

    public Cart() { }

    public Cart(int id)
    {
        if (id <= 0)
            throw new ArgumentException("El id del carrito es obligatorio", nameof(id));

        Id = id;
    }

    public void AddItem(Item item)
    {
        if (item is null)
            throw new ArgumentNullException("El item es obligatorio", nameof(item));

        var existingItem = _items.FirstOrDefault(i => i.Id == item.Id);

        if (existingItem != null)
        {
            existingItem.Quantity += item.Quantity;
        }
        else
        {
            _items.Add(item);
        }
    }

    public void RemoveItem(int itemId)
    {
        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
        {
            _items.Remove(item);
        }
    }
}
