namespace OnlineShopping.Api.Services;

using System.Drawing;
using Domain.Cart;
using Domain.Cart.Business.Entities;
using Domain.Cart.Business.Interfaces;
using Interfaces;
using OnlineShopping.Api.Dtos;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;

    public CartService(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public async Task<Cart>? GetByIdAsync(string id)
    {
        return await _cartRepository.GetByIdAsync(id);
    }

    public async Task<Cart>? GetAsync()
    {
        return await _cartRepository.GetAsync();
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

    public async Task<OperationResult> UpdateItemAsync(int itemId, ProductUpdatedEvent item)
    {
        try
        {
            var cart = await _cartRepository.GetAsync();
            if (cart is null)
            {
                return OperationResult.NotFound();
            }

            var existingItem = cart.Items.FirstOrDefault(i => i.Id == itemId);

            if (existingItem is null)
            {
                return OperationResult.NotFound();
            }

            ItemImage image = null;
            if (!string.IsNullOrEmpty(item.ImageUrl))
            {
                image = new ItemImage(item.ImageUrl, string.Empty);
            }

            existingItem.Update(item.Name, image, item.Price);

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
