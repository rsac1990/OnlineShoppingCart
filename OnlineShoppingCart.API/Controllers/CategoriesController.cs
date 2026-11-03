namespace OnlineShopping.Api.Controllers;

using Dtos;
using Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly LinkGenerator _linkGenerator;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(ICategoryService categoryService,
                                LinkGenerator linkGenerator,
                                ILogger<CategoriesController> logger)
    {
        _categoryService = categoryService;
        _linkGenerator = linkGenerator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetList()
    {
        try
        {
            var categories = await _categoryService.GetAllAsync();

            foreach (var category in categories)
            {
                GenerateLinks(category);
            }

            return Ok(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al obtener la información: {ex.Message}");
            return ServerError();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CategoryDto category)
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
    public async Task<IActionResult> Update(int id, CategoryDto category)
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

    private void GenerateLinks(CategoryDto category)
    {
        var controllerName = GetType().Name.Replace("Controller", "");

        category.Links.Add(new LinkDto
        {
            Href = _linkGenerator.GetUriByAction(
                HttpContext,
                action: nameof(Update),
                controller: controllerName,
                values: new { id = category.Id }) ?? string.Empty,
            Rel = "update",
            Method = "PUT"
        });

        category.Links.Add(new LinkDto
        {
            Href = _linkGenerator.GetUriByAction(
                HttpContext,
                action: nameof(Delete),
                controller: controllerName,
                values: new { id = category.Id }) ?? string.Empty,
            Rel = "delete",
            Method = "DELETE"
        });
    }
}
