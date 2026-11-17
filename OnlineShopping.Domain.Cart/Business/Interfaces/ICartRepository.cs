namespace OnlineShopping.Domain.Cart.Business.Interfaces;

using System.Threading.Tasks;
using Entities;

public interface ICartRepository
{
    Task<Cart> GetByIdAsync(string cartId);
    Task<Cart> GetAsync();
    Task UpdateAsync(Cart cart);
}
