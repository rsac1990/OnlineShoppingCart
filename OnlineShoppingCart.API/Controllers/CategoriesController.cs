namespace OnlineShoppingCart.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using OnlineShopping.Domain.Catalog.Entities;
using OnlineShopping.Domain.Catalog.Interfaces;

[ApiController]
[Route("[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger)
    {
        _categoryService = categoryService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetList()
    {
        try
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error ocurred when retrieving the Categories");
            return StatusCode(500, new
            {
                success = false,
                message = $"An error occurred while retrieving categories: {ex.Message}"
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Category>> Get(int id)
    {
        try
        {
            var category = await _categoryService.GetAsync(id);

            if (category is null)
            {
                return NotFound($"Category with id {id} not found.");
            }

            return Ok(category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error ocurred when retrieving the Category {id}");
            return StatusCode(500, new
            {
                success = false,
                message = $"An error occurred while retrieving categories: {ex.Message}"
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] Category category)
    {
        try
        {
            await _categoryService.AddAsync(category);
            return Ok("Category added successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error ocurred when adding the category {category.Id}");
            return StatusCode(500, new
            {
                success = false,
                message = $"An error ocurred when adding the category {category.Id}"
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Category category)
    {
        if (id != category.Id)
        {
            return BadRequest();
        }

        try
        {
            await _categoryService.UpdateAsync(id, category);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error ocurred when saving the category {category.Id}");

            return StatusCode(500, new
            {
                success = false,
                message = $"An error ocurred when saving the category {category.Id}"
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodoItem(int id)
    {
        try
        {
            var category = await _categoryService.GetAsync(id);
            if (category is null)
            {
                return NotFound();
            }

            await _categoryService.DeleteAsync(id);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error ocurred when deleting the category {id}");
            return StatusCode(500, new
            {
                success = false,
                message = $"An error ocurred when deleting the category {id}"
            });
        }
    }
}
