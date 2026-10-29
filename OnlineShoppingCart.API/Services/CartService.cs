namespace OnlineShopping.Api.Services;

using OnlineShopping.Domain.Cart.Business.Entities;
using OnlineShopping.Domain.Cart.Business.Interfaces;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;

    public CartService(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public async Task<List<Item>> GetItems(int cartId)
    {
        var cart = await _cartRepository.GetByIdAsync(cartId);
        return cart?.Items.ToList() ?? new List<Item>();
    }

    public async Task<(bool Success, string ErrorMessage)> AddItemAsync(int cartId, Item newItem)
    {
        var cart = await _cartRepository.GetByIdAsync(cartId);
        if (cart == null)
        {
            return (false, "Carrito no encontrado");
        }

        cart.AddItem(newItem);

        await _cartRepository.UpdateAsync(cart);
        return (true, string.Empty);
    }

    public async Task<(bool Success, string ErrorMessage)> RemoveItem(int cartId, int itemId)
    {
        var cart = await _cartRepository.GetByIdAsync(cartId);
        if (cart == null)
        {
            return (false, "Carrito no encontrado");
        }

        cart.RemoveItem(itemId);

        await _cartRepository.UpdateAsync(cart);
        return (true, string.Empty);
    }
}
