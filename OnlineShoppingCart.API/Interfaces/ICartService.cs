namespace OnlineShopping.Api.Interfaces;

using Domain.Cart.Business.Entities;

public interface ICartService
{
    Task<Cart>? GetAsync(string id);
    Task<OperationResult> AddItemAsync(string cartId, Item newItem);
    Task<OperationResult> RemoveItem(string cartId, int itemId);
}