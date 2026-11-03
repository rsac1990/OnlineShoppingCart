namespace OnlineShopping.Api.Controllers;

using Domain.Cart.Business.Entities;
using Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class CartsController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly ILogger<CartsController> _logger;

    public CartsController(ICartService cartService, ILogger<CartsController> logger)
    {
        _cartService = cartService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<Cart>> Get(string id)
    {
        try
        {
            var cart = await _cartService.GetAsync(id);

            if (cart is null)
            {
                return NotFound();
            }

            return Ok(cart);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al obtener la información: {ex.Message}");
            return ServerError();
        }
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> AddItem(string cartId, [FromBody] Item item)
    {
        try
        {
            var result = await _cartService.AddItemAsync(cartId, item);

            if (result.ErrorType == ErrorType.Validation)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok("Item agregado con éxito");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al guardar la información: {ex.Message}");
            return ServerError();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteItem(string cartId, int itemId)
    {
        try
        {
            var result = await _cartService.RemoveItem(cartId, itemId);
            if (result.ErrorType == ErrorType.NotFound)
            {
                return NotFound();
            }

            return Ok("Item eliminado con éxito");
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
