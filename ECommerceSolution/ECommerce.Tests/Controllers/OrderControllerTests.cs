using ECommerce.API.Controllers;
using ECommerce.Application.DTOs.Order;
using ECommerce.Application.Interfaces.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace ECommerce.Tests.Controllers
{
    public class OrderControllerTests
    {
        private readonly Mock<IOrderService> _orderServiceMock;
        private readonly OrderController _controller;

        public OrderControllerTests()
        {
            _orderServiceMock = new Mock<IOrderService>();
            _controller = new OrderController(_orderServiceMock.Object);
        }

        private void SetUser(int userId)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task GetMyOrders_ReturnOk()
        {
            // Arrange
            SetUser(1);

            var orders = new List<OrderDto>();

            _orderServiceMock.Setup(x => x.GetUserOrdersAsync(1)).ReturnsAsync(orders);

            // Act
            var result = await _controller.GetMyOrders();

            // Assert
            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(orders);
        }

        [Fact]
        public async Task InitiateCheckout_ReturnOk_WhenValidRequest()
        {
            // Arrange
            SetUser(1);

            var dto = new CheckoutRequestDto();

            var checkoutSessionDto = new CheckoutSessionDto();

            _orderServiceMock.Setup(x => x.InitiateCheckoutAsync(1, dto, It.IsAny<CancellationToken>())).ReturnsAsync(checkoutSessionDto);

            // Act
            var actual = await _controller.InitiateCheckout(dto, It.IsAny<CancellationToken>());

            // Assert
            var ok = actual.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(checkoutSessionDto);
        }

        [Fact]
        public async Task InitiateCheckout_ReturnBadRequest_WhenInvalidOperation()
        {
            // Arrange
            SetUser(1);

            var dto = new CheckoutRequestDto();

            _orderServiceMock.Setup(x => x.InitiateCheckoutAsync(1, dto, It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException("Cart is empty"));

            // Act
            var actual = await _controller.InitiateCheckout(dto, It.IsAny<CancellationToken>());

            // Assert
            var bad = actual.Should().BeOfType<BadRequestObjectResult>().Subject;
            bad.Value.Should().NotBeNull();
        }


        [Fact]
        public async Task ProcessResponseCallback_ReturnOk_WhenSuccess()
        {
            // Arrange
            var callback = new PaymentCallbackDto();

            var paymentResultDto = new PaymentResultDto();

            _orderServiceMock.Setup(x => x.ProcessPaymentCallbackAsync(callback, It.IsAny<CancellationToken>())).ReturnsAsync(paymentResultDto);

            // Act
            var actual = await _controller.ProcessResponseCallback(callback, It.IsAny<CancellationToken>());

            // Assert
            var ok = actual.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(paymentResultDto);
        }

        [Fact]
        public async Task ProcessResponseCallback_ReturnBadRequest_WhenInvalidOperation()
        {
            // Arrange
            var callback = new PaymentCallbackDto();

            _orderServiceMock.Setup(x => x.ProcessPaymentCallbackAsync(callback, It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException("Invalid signature"));

            // Act
            var actual = await _controller.ProcessResponseCallback(callback, It.IsAny<CancellationToken>());

            // Assert
            var bad = actual.Should().BeOfType<BadRequestObjectResult>().Subject;
            bad.Value.Should().NotBeNull();
        }


    }
}
