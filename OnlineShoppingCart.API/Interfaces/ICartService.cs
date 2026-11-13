namespace OnlineShopping.Api.Interfaces;

using Domain.Cart.Business.Entities;
using OnlineShopping.Api.Dtos;

public interface ICartService
{
    Task<Cart>? GetByIdAsync(string id);
    Task<Cart>? GetAsync();
    Task<OperationResult> AddItemAsync(string cartId, Item newItem);
    Task<OperationResult> UpdateItemAsync(int itemId, ProductUpdatedEvent newItem);
    Task<OperationResult> RemoveItem(string cartId, int itemId);
}