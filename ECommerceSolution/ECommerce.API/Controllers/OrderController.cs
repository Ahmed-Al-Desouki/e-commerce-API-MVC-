using ECommerce.API.Helpers;
using ECommerce.Application.DTOs.Order;
using ECommerce.Application.Interfaces.Services;
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

        [HttpGet]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = ClaimsHelper.GetUserId(User);
            var orders = await _orderService.GetUserOrdersAsync(userId);
            return Ok(orders);
        }

        [HttpPost("checkout/initiate")]
        public async Task<IActionResult> InitiateCheckout([FromBody] CheckoutRequestDto request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = ClaimsHelper.GetUserId(User);
                var session = await _orderService.InitiateCheckoutAsync(userId, request, cancellationToken);
                return Ok(session);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("payment/response-callback")]
        public async Task<IActionResult> ProcessResponseCallback([FromBody] PaymentCallbackDto callback, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _orderService.ProcessPaymentCallbackAsync(callback, cancellationToken);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
