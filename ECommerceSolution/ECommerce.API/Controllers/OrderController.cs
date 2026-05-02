using ECommerce.API.Helpers;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>Get all orders placed by the current user.</summary>
        [HttpGet]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = ClaimsHelper.GetUserId(User);
            var orders = await _orderService.GetUserOrdersAsync(userId);
            return Ok(orders);
        }

        /// <summary>Checkout — converts the current cart into an order.</summary>
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout()
        {
            try
            {
                var userId = ClaimsHelper.GetUserId(User);
                var order = await _orderService.CheckoutAsync(userId);
                return Ok(order);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
