namespace TestProject.IntegrationTests;

using System.Threading.Tasks;
using FluentAssertions;
using Mongo2Go;
using MongoDB.Driver;
using NUnit.Framework;
using OnlineShopping.Domain.Cart.Business.Entities;
using OnlineShopping.Domain.Cart.Business.Services;
using OnlineShopping.Domain.Cart.Data;
using OnlineShopping.Domain.Cart.Data.Repositories;

[TestFixture]
public class CartServiceTests
{
    private MongoDbRunner _runner;
    private DBContext _dbContext;
    private CartRepository _cartRepository;
    private CartService _cartService;

    private Item _cartItem;
    private readonly int _testCartId = 1;

    [OneTimeSetUp]
    public void GlobalSetUp()
    {
        _runner = MongoDbRunner.Start();

       _dbContext = new DBContext(_runner.ConnectionString, "OnlineShoppingTestDb");
        _cartRepository = new CartRepository(_dbContext);
        _cartService = new CartService(_cartRepository);

        _cartItem = new Item(1,
                             "Test Product",
                             new ItemImage("https://example.com/image.png", "Product Image"),
                             19,
                             2);
    }

    [OneTimeTearDown]
    public void GlobalTearDown()
    {
        // Clean up and stop the MongoDB process when tests finish
        _runner?.Dispose();
    }

    [SetUp]
    public async Task CleanUpBeforeTest()
    {
        await _dbContext.Carts.DeleteManyAsync(c => c.Id == _testCartId);
    }

    [Test]
    public async Task AddItemAsync_ShouldAddItemToExistingCart()
    {
        await _dbContext.Carts.InsertOneAsync(new Cart(_testCartId));

        await _cartService.AddItemAsync(_testCartId, _cartItem);

        var cartInDb = await _dbContext.Carts.Find(c => c.Id == _testCartId).FirstOrDefaultAsync();

        cartInDb.Should().NotBeNull();
        cartInDb.Items.Should().ContainSingle();
        var cartDbItems = cartInDb.Items.ToList();
        cartDbItems[0].Name.Should().Be("Test Product");
        cartDbItems[0].Quantity.Should().Be(2);
        cartDbItems[0].Price.Should().Be(19);
    }

    [Test]
    public async Task GetItems_ShouldGETItemsFromExistingCart()
    {
        var cart= new Cart(_testCartId);
        cart.AddItem(_cartItem);

        await _dbContext.Carts.InsertOneAsync(cart);

        var cartDbItems = await _cartService.GetItems(_testCartId);

        cartDbItems[0].Name.Should().Be("Test Product");
        cartDbItems[0].Quantity.Should().Be(2);
        cartDbItems[0].Price.Should().Be(19);
    }
}
