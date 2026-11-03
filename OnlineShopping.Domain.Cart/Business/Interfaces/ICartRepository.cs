namespace OnlineShopping.Domain.Cart.Business.Interfaces;

using System.Threading.Tasks;
using Entities;

public interface ICartRepository
{
    Task<Cart> GetByIdAsync(string cartId);
    Task UpdateAsync(Cart cart);
}
