using ECommerce.API.Helpers;
using ECommerce.Application.DTOs.Cart;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        /// <summary>Get the current user's cart.</summary>
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = ClaimsHelper.GetUserId(User);
            var cart = await _cartService.GetCartAsync(userId);
            return Ok(cart);
        }

        /// <summary>Add a product to the current user's cart.</summary>
        [HttpPost("items")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            try
            {
                var userId = ClaimsHelper.GetUserId(User);
                await _cartService.AddToCartAsync(userId, dto);
                return Ok(new { message = "Item added to cart." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>Clear all items from the current user's cart.</summary>
        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            var userId = ClaimsHelper.GetUserId(User);
            await _cartService.ClearCartAsync(userId);
            return Ok(new { message = "Cart cleared." });
        }
    }
}
