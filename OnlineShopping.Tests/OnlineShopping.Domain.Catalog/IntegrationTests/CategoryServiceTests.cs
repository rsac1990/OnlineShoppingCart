namespace OnlineShopping.Tests.OnlineShopping.Domain.Catalog.IntegrationTests;

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using OnlineShopping.Data.Catalog;
using OnlineShopping.Data.Catalog.Repositories;
using OnlineShopping.Domain.Catalog.Entities;
using OnlineShopping.Domain.Catalog.Services;

[TestFixture]
public class CategoryServiceTests
{
    private AppDbContext _context;
    private Repository<Category> _repository;
    private CategoryService _service;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();

        _repository = new Repository<Category>(_context);
        _service = new CategoryService(_repository);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task AddCategory_ShouldPersistCategory_WhenDataIsValid()
    {
        var category = new Category("Electronics");

        await _service.AddAsync(category);
        var categories = await _service.GetAllAsync();

        categories.Should().ContainSingle()
            .Which.Name.Should().Be("Electronics");
    }

    [Test]
    public async Task GetCategory_ShouldReturnCategory_WhenIdExists()
    {
        var category = new Category("Books");
        await _service.AddAsync(category);

        var retrieved = await _service.GetAsync(category.Id);

        retrieved.Should().NotBeNull();
        retrieved!.Name.Should().Be("Books");
    }

    [Test]
    public async Task GetCategory_ShouldReturnNull_WhenIdDoesNotExist()
    {
        var retrieved = await _service.GetAsync(999);

        retrieved.Should().BeNull();
    }

    [Test]
    public async Task UpdateCategory_ShouldModifyExistingCategory_WhenIdExists()
    {
        var category = new Category("Toys");
        await _service.AddAsync(category);

        var updatedCategory = new Category("Toys & Games");
        await _service.UpdateAsync(category.Id, updatedCategory);

        var retrieved = await _service.GetAsync(category.Id);

        retrieved!.Name.Should().Be("Toys & Games");
    }

    [Test]
    public async Task DeleteCategory_ShouldRemoveCategory_WhenIdExists()
    {
        var category = new Category("Clothing");
        await _service.AddAsync(category);

        await _service.DeleteAsync(category.Id);
        var categories = await _service.GetAllAsync();

        categories.Should().BeEmpty();
    }
}