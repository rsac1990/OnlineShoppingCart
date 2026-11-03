namespace TestProject.IntegrationTests.Api.Services;

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using OnlineShopping.Api.Dtos;
using OnlineShopping.Api.Services;
using OnlineShopping.Data.Catalog;
using OnlineShopping.Data.Catalog.Repositories;
using OnlineShopping.Domain.Catalog.Entities;

[TestFixture]
public class ProductServiceTests
{
    private AppDbContext _context;
    private ProductRepository _productRepository;
    private ProductService _service;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();

        _productRepository = new ProductRepository(_context);
        _service = new ProductService(_productRepository);

        SeedCategories();
    }

    private Task SeedCategories()
    {
        var category1 = new Category("Electronics");
        category1.Id = 1; // Manually set ID for testing purposes

        var category2 = new Category("Clothing");
        category2.Id = 2; // Manually set ID for testing purposes
        
        var categories = new List<Category>
        {
            category1,
            category2
        };
        _context.Categories.AddRange(categories);
        return _context.SaveChangesAsync();
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task AddProduct_ShouldPersistProduct_WhenDataIsValid()
    {
        var productDto = new ProductDto
        {
            Name = "Laptop",
            CategoryId = 1,
            Price = 999.99m,
            Amount = 10,
            Description = "High-end gaming laptop"
        };

        var result = await _service.AddAsync(productDto);
        result.IsSuccess.Should().BeTrue();

        var pagedResult = await _service.GetListAsync(new ProductParametersDto());

        pagedResult.Items.Should().ContainSingle()
            .Which.Name.Should().Be("Laptop");
    }

    [Test]
    public async Task GetList_ShouldRespectPagination_WhenParametersAreProvided()
    {
        // Seed 3 products
        for (int i = 1; i <= 3; i++)
        {
            await _service.AddAsync(new ProductDto
            {
                Name = $"Product {i}",
                CategoryId = 1,
                Price = 10 * i,
                Amount = 5
            });
        }

        var parameters = new ProductParametersDto
        {
            PageNumber = 2,
            PageSize = 1
        };

        var pagedResult = await _service.GetListAsync(parameters);

        pagedResult.TotalCount.Should().Be(3);
        pagedResult.PageNumber.Should().Be(2);
        pagedResult.PageSize.Should().Be(1);
        pagedResult.Items.Should().ContainSingle()
            .Which.Name.Should().Be("Product 2");
    }

    [Test]
    public async Task GetList_ShouldFilterByCategory_WhenCategoryIdIsProvided()
    {
        // Seed products with different category IDs
        await _service.AddAsync(new ProductDto { Name = "Shirt", CategoryId = 1, Price = 20m, Amount = 5 });
        await _service.AddAsync(new ProductDto { Name = "Pants", CategoryId = 1, Price = 30m, Amount = 5 });
        await _service.AddAsync(new ProductDto { Name = "Phone", CategoryId = 2, Price = 500m, Amount = 2 });

        var parameters = new ProductParametersDto
        {
            CategoryId = 1
        };

        var pagedResult = await _service.GetListAsync(parameters);

        pagedResult.TotalCount.Should().Be(2);
        pagedResult.Items.Should().HaveCount(2);
        pagedResult.Items.All(p => p.CategoryId == 1).Should().BeTrue();
    }

    [Test]
    public async Task UpdateProduct_ShouldModifyExistingProduct_WhenIdExists()
    {
        var productDto = new ProductDto
        {
            Name = "Old Name",
            CategoryId = 1,
            Price = 100m,
            Amount = 5
        };
        await _service.AddAsync(productDto);

        var pagedResult = await _service.GetListAsync(new ProductParametersDto());
        var existingProduct = pagedResult.Items.First();

        var updatedDto = new ProductDto
        {
            Name = "New Name",
            CategoryId = 1,
            Price = 150m,
            Amount = 8
        };

        var result = await _service.UpdateAsync(existingProduct.Id, updatedDto);
        result.IsSuccess.Should().BeTrue();

        var retrievedList = await _service.GetListAsync(new ProductParametersDto());
        var retrieved = retrievedList.Items.FirstOrDefault(p => p.Id == existingProduct.Id);

        retrieved.Should().NotBeNull();
        retrieved!.Name.Should().Be("New Name");
        retrieved.Price.Should().Be(150m);
    }

    [Test]
    public async Task DeleteProduct_ShouldRemoveProduct_WhenIdExists()
    {
        var productDto = new ProductDto
        {
            Name = "ToDelete",
            CategoryId = 1,
            Price = 50m,
            Amount = 2
        };
        await _service.AddAsync(productDto);

        var pagedResult = await _service.GetListAsync(new ProductParametersDto());
        var existingProduct = pagedResult.Items.First();

        var result = await _service.DeleteAsync(existingProduct.Id);
        result.IsSuccess.Should().BeTrue();

        var remaining = await _service.GetListAsync(new ProductParametersDto());
        remaining.TotalCount.Should().Be(0);
        remaining.Items.Should().BeEmpty();
    }
}
