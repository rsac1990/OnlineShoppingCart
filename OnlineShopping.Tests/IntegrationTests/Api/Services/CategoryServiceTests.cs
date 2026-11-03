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
        var categoryDto = new CategoryDto
        {
            Name = "Electronics"
        };

        var result = await _service.AddAsync(categoryDto);
        result.IsSuccess.Should().BeTrue();

        var categories = await _service.GetAllAsync();

        categories.Should().ContainSingle()
            .Which.Name.Should().Be("Electronics");
    }

    [Test]
    public async Task GetCategory_ShouldReturnCategory_WhenIdExists()
    {
        var categoryDto = new CategoryDto
        {
            Name = "Books"
        };
        await _service.AddAsync(categoryDto);

        var categories = await _service.GetAllAsync();
        var addedCategory = categories.FirstOrDefault(c => c.Name == "Books");

        addedCategory.Should().NotBeNull();
        addedCategory!.Name.Should().Be("Books");
    }

    [Test]
    public async Task GetCategory_ShouldReturnNullOrNotFound_WhenIdDoesNotExist()
    {
        var categories = await _service.GetAllAsync();
        categories.FirstOrDefault(c => c.Id == 999).Should().BeNull();
    }

    [Test]
    public async Task UpdateCategory_ShouldModifyExistingCategory_WhenIdExists()
    {
        var categoryDto = new CategoryDto
        {
            Name = "Books"
        };
        await _service.AddAsync(categoryDto);

        var categories = await _service.GetAllAsync();
        var existingCategory = categories.First(c => c.Name == "Books");

        var updatedCategoryDto = new CategoryDto
        {
            Name = "Toys & Games"
        };

        var result = await _service.UpdateAsync(existingCategory.Id, updatedCategoryDto);
        result.IsSuccess.Should().BeTrue();

        var retrievedList = await _service.GetAllAsync();
        var retrieved = retrievedList.FirstOrDefault(c => c.Id == existingCategory.Id);

        retrieved.Should().NotBeNull();
        retrieved!.Name.Should().Be("Toys & Games");
    }

    [Test]
    public async Task DeleteCategory_ShouldRemoveCategory_WhenIdExists()
    {
        var categoryDto = new CategoryDto
        {
            Name = "Clothing"
        };
        await _service.AddAsync(categoryDto);

        var categories = await _service.GetAllAsync();
        var existingCategory = categories.First(c => c.Name == "Clothing");

        var result = await _service.DeleteAsync(existingCategory.Id);
        result.IsSuccess.Should().BeTrue();

        var remainingCategories = await _service.GetAllAsync();
        remainingCategories.Should().BeEmpty();
    }
}