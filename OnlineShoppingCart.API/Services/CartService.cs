namespace OnlineShopping.Api.Services;

using Domain.Cart;
using Domain.Cart.Business.Entities;
using Domain.Cart.Business.Interfaces;
using Interfaces;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;

    public CartService(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public async Task<Cart>? GetAsync(string id)
    {
        return await _cartRepository.GetByIdAsync(id);
    }

    public async Task<OperationResult> AddItemAsync(string cartId, Item newItem)
    {
        try
        {
            var cart = await _cartRepository.GetByIdAsync(cartId);
            if (cart is null)
            {
                cart = new Cart(cartId);
            }

            cart.AddItem(newItem);

            await _cartRepository.UpdateAsync(cart);
            return OperationResult.Success();
        }
        catch (DomainException ex)
        {
            return OperationResult.Failure(ex.Message, ErrorType.Validation);
        }
    }

    public async Task<OperationResult> RemoveItem(string cartId, int itemId)
    {
        var cart = await _cartRepository.GetByIdAsync(cartId);
        if (cart is null)
        {
            return OperationResult.NotFound($"Carrito {cartId} no encontrado");
        }

        cart.RemoveItem(itemId);

        await _cartRepository.UpdateAsync(cart);
        return OperationResult.Success();
    }

}
