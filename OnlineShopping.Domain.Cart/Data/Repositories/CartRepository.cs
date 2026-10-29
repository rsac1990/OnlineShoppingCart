namespace OnlineShopping.Domain.Cart.Data.Repositories;

using MongoDB.Driver;
using OnlineShopping.Domain.Cart.Business.Entities;
using OnlineShopping.Domain.Cart.Business.Interfaces;

public class CartRepository : ICartRepository
{
    private readonly DBContext _context;

    public CartRepository(DBContext context)
    {
        _context = context;
    }

    public async Task<Cart> GetByIdAsync(int cartId)
    {
        return await _context.Carts
            .Find(c => c.Id == cartId)
            .FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(Cart cart)
    {
        var filter = Builders<Cart>.Filter.Eq(c => c.Id, cart.Id);
        var options = new ReplaceOptions { IsUpsert = false }; 

        await _context.Carts.ReplaceOneAsync(filter, cart, options);
    }
}
