namespace OnlineShopping.Domain.Cart.Business.Interfaces;

using Entities;

public interface ICartService
{
    Task<Cart>? GetAsync(string id);
    Task<(bool Success, string ErrorMessage)> AddItemAsync(int cartId, Item newItem);
    Task<(bool Success, string ErrorMessage)> RemoveItem(int cartId, int itemId);
}