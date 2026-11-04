namespace OnlineShopping.Api.Controllers;

using Asp.Versioning;
using Domain.Cart.Business.Entities;
using Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("v{version:apiVersion}/Carts")]
public class CartsController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly ILogger<CartsController> _logger;

    public CartsController(ICartService cartService, ILogger<CartsController> logger)
    {
        _cartService = cartService;
        _logger = logger;
    }

    [HttpGet("{id}")]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<Cart>> GetV1(string id)
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


    [HttpGet("{id}")]
    [MapToApiVersion("2.0")]
    public async Task<ActionResult<List<Item>>> GetV2(string id)
    {
        try
        {
            var cart = await _cartService.GetAsync(id);

            if (cart is null)
            {
                return NotFound();
            }

            return Ok(cart.Items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al obtener la información: {ex.Message}");
            return ServerError();
        }
    }

    [HttpPost("{cartId}/items")]
    [MapToApiVersion("1.0")]
    [MapToApiVersion("2.0")]
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

    [HttpDelete("{cartId}/items/{itemId}")]
    [MapToApiVersion("1.0")]
    [MapToApiVersion("2.0")]
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
