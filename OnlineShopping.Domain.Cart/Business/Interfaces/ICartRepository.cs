namespace OnlineShopping.Domain.Cart.Business.Interfaces;

using System.Threading.Tasks;
using OnlineShopping.Domain.Cart.Business.Entities;

public interface ICartRepository
{
    Task<Cart> GetByIdAsync(int cartId);
    Task UpdateAsync(Cart cart);
}
