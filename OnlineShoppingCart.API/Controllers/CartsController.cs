namespace OnlineShopping.Api.Controllers;

using Asp.Versioning;
using Domain.Cart.Business.Entities;
using Interfaces;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Operations for managing shopping carts.
/// </summary>
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

    /// <summary>
    /// Gets a cart with all its details (version 1.0).
    /// </summary>
    /// <param name="id">The unique identifier of the cart.</param>
    /// <returns>The cart that matches the identifier.</returns>
    /// <response code="200">The cart was found.</response>
    /// <response code="404">No cart exists with the given identifier.</response>
    /// <response code="500">An unexpected server error occurred.</response>
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

    /// <summary>
    /// Gets only the items of a cart (version 2.0).
    /// </summary>
    /// <param name="id">The unique identifier of the cart.</param>
    /// <response code="200">The list of items in the cart.</response>
    /// <response code="404">No cart exists with the given identifier.</response>
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

    /// <summary>
    /// Adds an item to a cart.
    /// </summary>
    /// <param name="cartId">The cart identifier.</param>
    /// <param name="item">The item to add.</param>
    /// <response code="200">The item was added.</response>
    /// <response code="400">The item failed validation.</response>
    [HttpPost("{cartId}/items")]
    //[MapToApiVersion("1.0")]
    //[MapToApiVersion("2.0")]
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

    /// <summary>
    /// deletes an item to a cart.
    /// </summary>
    /// <param name="cartId">The cart identifier.</param>
    /// <param name="itemId">The item to remove.</param>
    /// <response code="200">The item was removed.</response>
    /// <response code="404">The item failed validation.</response>
    [HttpDelete("{cartId}/items/{itemId}")]
    //[MapToApiVersion("1.0")]
    //[MapToApiVersion("2.0")]
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
