namespace TestProject.Cart.UnitTests.Bussiness.Entities;

using FluentAssertions;
using NUnit.Framework;
using OnlineShopping.Domain.Cart.Business.Entities;

[TestFixture]
public class ItemTests
{
    [Test]
    public void Constructor_WithValidArguments_ShouldCreateItemSuccessfully()
    {
        int id = 1;
        string name = "Laptop Gamer";
        var image = new ItemImage("https://example.com/image.jpg", "Laptop frontal");
        decimal price = 1500;
        int quantity = 2;

        var item = new Item(id, name, image, price, quantity);

        item.Id.Should().Be(id);
        item.Name.Should().Be(name);
        item.Image.Should().Be(image);
        item.Price.Should().Be(price);
        item.Quantity.Should().Be(quantity);
    }

    [Test]
    [TestCase(0)]
    [TestCase(-1)]
    public void Constructor_WithInvalidId_ShouldThrowException(int invalidId)
    {
        string name = "Laptop Gamer";
        var image = new ItemImage("https://example.com/image.jpg", "Laptop frontal");
        decimal price = 1500;
        int quantity = 2;

        Action act = () => new Item(invalidId, name, image, price, quantity);

        act.Should().Throw<ArgumentException>()
           .WithMessage("*El id del item es obligatorio*")
           .WithParameterName("id");
    }

    [Test]
    public void Constructor_WithInvalidPrice_ShouldThrowException()
    {
        int id = 1;
        string name = "Laptop Gamer";
        var image = new ItemImage("https://example.com/image.jpg", "Laptop frontal");
        decimal price = -1500;
        int quantity = 2;

        Action act = () => new Item(id, name, image, price, quantity);

        act.Should().Throw<ArgumentException>()
           .WithMessage("*El precio no puede ser negativo*")
           .WithParameterName("price");
    }

    [Test]
    public void Constructor_WithInvalidQuantity_ShouldThrowException()
    {
        int id = 1;
        string name = "Laptop Gamer";
        var image = new ItemImage("https://example.com/image.jpg", "Laptop frontal");
        decimal price = 1500;
        int quantity = -2;

        Action act = () => new Item(id, name, image, price, quantity);

        act.Should().Throw<ArgumentException>()
           .WithMessage("*La cantidad no puede ser negativa*")
           .WithParameterName("quantity");
    }
}