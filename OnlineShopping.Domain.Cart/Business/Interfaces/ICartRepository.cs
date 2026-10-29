namespace OnlineShopping.Task1.Business.Interfaces;

using System.Threading.Tasks;
using OnlineShopping.Task1.Business.Entities;

public interface ICartRepository
{
    Task<Cart> GetByIdAsync(int cartId);
    Task UpdateAsync(Cart cart);
}
