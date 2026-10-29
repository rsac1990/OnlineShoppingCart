namespace TestProject.Catalog.UnitTests.Entities;

using FluentAssertions;
using NUnit.Framework;
using OnlineShopping.Domain.Catalog;
using OnlineShopping.Domain.Catalog.Entities;

[TestFixture]
public class CategoryTests
{
    [Test]
    public void Constructor_ShouldCreateCategory_WhenNameIsValid()
    {
        var category = new Category("Electronics");

        category.Name.Should().Be("Electronics");
        category.Image.Should().BeNull();
        category.ParentCategory.Should().BeNull();
    }


    [TestCase("")]
    [TestCase("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
    public void Constructor_ShouldThrowDomainException_WhenNameIsInvalid(string invalidName)
    {
        var act = () => new Category(invalidName!);

        act.Should().Throw<DomainException>()
            .WithMessage("Nombre inválido, debe tener máximo 50 caracteres");
    }

    [Test]
    public void SetImage_ShouldSetImage_WhenUrlIsValid()
    {
        var category = new Category("Electronics");

        category.SetImage("https://example.com/image.png");

        category.Image.Should().Be("https://example.com/image.png");
    }

    [Test]
    public void SetImage_ShouldThrowDomainException_WhenUrlIsInvalid()
    {
        var category = new Category("Electronics");

        var act = () => category.SetImage("not-a-valid-url");

        act.Should().Throw<DomainException>()
            .WithMessage("La URL de la imagen no es válida.");
    }
}