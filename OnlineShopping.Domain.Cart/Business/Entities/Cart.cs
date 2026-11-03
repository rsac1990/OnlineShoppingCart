namespace OnlineShopping.Domain.Cart.Business.Entities;

public class Cart : Entity
{
    private List<Item> _items = new();

    public IReadOnlyCollection<Item> Items => _items.AsReadOnly();

    public Cart() { }

    public Cart(string id)
    {
        if (string.IsNullOrEmpty(id))
             throw new DomainException("El id del carrito es obligatorio");

        Id = id;
    }

    public void AddItem(Item item)
    {
        if (item is null)
            throw new DomainException("El item es obligatorio");

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

    public void RemoveItem(string itemId)
    {
        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
        {
            _items.Remove(item);
        }
    }
}
