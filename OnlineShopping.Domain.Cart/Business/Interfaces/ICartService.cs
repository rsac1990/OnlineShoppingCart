namespace OnlineShopping.Domain.Cart.Business.Interfaces;

using OnlineShopping.Domain.Cart.Business.Entities;

public interface ICartService
{
    Task<(bool Success, string ErrorMessage)> AddItemAsync(int cartId, Item newItem);
    Task<List<Item>> GetItems(int cartId);
    Task<(bool Success, string ErrorMessage)> RemoveItem(int cartId, int itemId);
}