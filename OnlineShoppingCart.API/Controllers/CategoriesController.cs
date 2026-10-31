namespace OnlineShoppingCart.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using OnlineShopping.Api;
using OnlineShopping.Api.Interfaces;
using OnlineShopping.Domain.Catalog.Entities;

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
            _logger.LogError(ex, $"Error al obtener la información: {ex.Message}");
            return ServerError();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] Category category)
    {
        try
        {
            var result = await _categoryService.AddAsync(category);
            
            if (result.ErrorType == ErrorType.Validation)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok("Categoría agregada con éxito");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al guardar la información: {ex.Message}");
            return ServerError();
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Category category)
    {
        try
        {
            var result = await _categoryService.UpdateAsync(id, category);

            if (result.ErrorType == ErrorType.NotFound)
            {
                return NotFound();
            }

            if (result.ErrorType == ErrorType.Validation)
            {
                return BadRequest(result.ErrorMessage);
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al actualizar la información: {ex.Message}");
            return ServerError();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _categoryService.DeleteAsync(id);
            if (result.ErrorType == ErrorType.NotFound)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al eliminar la información: {ex.Message}");
            return ServerError();
        }
    }

    private ObjectResult ServerError()
    {
        return StatusCode(500, new
        {
            success = false,
            message = $"Error en el servidor, contactar al administrador del sitio"
        });
    }
}
