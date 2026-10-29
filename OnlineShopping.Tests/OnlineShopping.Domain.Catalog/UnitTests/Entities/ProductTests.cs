namespace OnlineShopping.Tests.OnlineShopping.Domain.Catalog.UnitTests.Entities;

using FluentAssertions;
using NUnit.Framework;
using OnlineShopping.Domain.Catalog;
using OnlineShopping.Domain.Catalog.Entities;

[TestFixture]
public class ProductTests
{
    private Category _category;
    private Product _product;

    [SetUp]
    public void SetUp()
    {
        _category = new Category("Electronics");
        _product = new Product("Laptop", _category, 1500, 10);
    }

    [Test]
    public void Constructor_ShouldCreateProduct_WhenDataIsValid()
    {
        _product.Name.Should().Be("Laptop");
        _product.Category.Should().Be(_category);
        _product.CategoryId.Should().Be(_category.Id);
        _product.Price.Should().Be(1500);
        _product.Amount.Should().Be(10);
        _product.Description.Should().BeNull();
        _product.Image.Should().BeNull();
    }

    [TestCase("")]
    [TestCase("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
    public void Constructor_ShouldThrowDomainException_WhenNameIsInvalid(string invalidName)
    {
        var act = () => new Product(invalidName, _category, 100, 5);

        act.Should().Throw<DomainException>()
            .WithMessage("Nombre inválido, debe tener máximo 50 caracteres");
    }

    [Test]
    public void Constructor_ShouldThrowDomainException_WhenCategoryIsNull()
    {
        var act = () => new Product("Laptop", null!, 100, 5);

        act.Should().Throw<DomainException>()
            .WithMessage("La categoria es obligatoria");
    }

    [Test]
    public void Constructor_ShouldThrowDomainException_WhenPriceIsNegative()
    {
        var act = () => new Product("Laptop", _category, -1, 5);

        act.Should().Throw<DomainException>()
            .WithMessage("El precio no puede ser negativo");
    }

    [Test]
    public void Constructor_ShouldThrowDomainException_WhenAmountIsNegative()
    {
        var act = () => new Product("Laptop", _category, 100, -1);

        act.Should().Throw<DomainException>()
            .WithMessage("La cantidad no puede ser negativa");
    }

    [Test]
    public void SetDescription_ShouldSanitizeHtml_WhenDescriptionContainsScriptTag()
    {
        _product.SetDescription("<p>Great laptop</p><script>alert('xss')</script>");

        _product.Description.Should().Contain("<p>Great laptop</p>");
        _product.Description.Should().NotContain("<script>");
    }

    [Test]
    public void SetDescription_ShouldKeepSafeHtml_WhenDescriptionIsSafe()
    {
        _product.SetDescription("<b>Bold text</b>");

        _product.Description.Should().Be("<b>Bold text</b>");
    }
}
