namespace OnlineShopping.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using OnlineShopping.Domain.Catalog.Entities;
using OnlineShopping.Domain.Catalog.Interfaces;

[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService productService, ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetList()
    {
        try
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error ocurred when retrieving the products");
            return StatusCode(500, new
            {
                success = false,
                message = $"An error occurred while retrieving products: {ex.Message}"
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> Get(int id)
    {
        try
        {
            var category = await _productService.GetAsync(id);

            if (category is null)
            {
                return NotFound($"Product with id {id} not found.");
            }

            return Ok(category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error ocurred when retrieving the Product {id}");
            return StatusCode(500, new
            {
                success = false,
                message = $"An error occurred while retrieving products: {ex.Message}"
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] Product category)
    {
        try
        {
            await _productService.AddAsync(category);
            return Ok("Product added successfully");
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
    public async Task<IActionResult> Update(int id, Product category)
    {
        if (id != category.Id)
        {
            return BadRequest();
        }

        try
        {
            await _productService.UpdateAsync(id, category);
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
            var category = await _productService.GetAsync(id);
            if (category is null)
            {
                return NotFound();
            }
            await _productService.DeleteAsync(id);

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
