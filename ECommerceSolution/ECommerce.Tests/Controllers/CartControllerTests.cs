using ECommerce.API.Controllers;
using ECommerce.Application.DTOs.Cart;
using ECommerce.Application.Interfaces.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace ECommerce.Tests.Controllers
{
    public class CartControllerTests
    {
        private readonly Mock<ICartService> _cartServiceMock;
        private readonly CartController _cartController;

        public CartControllerTests()
        {
            _cartServiceMock = new Mock<ICartService>();
            _cartController = new CartController(_cartServiceMock.Object);
        }

        [Fact]
        public async Task GetCart_ReturnsOk_WhenCartisExisit()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            }));

            var Cart = new CartDto();

            _cartServiceMock.Setup(x => x.GetCartAsync(1)).ReturnsAsync(Cart);

            _cartController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var actual = await _cartController.GetCart();

            // Assert
            var ok = actual.Should().BeOfType<OkObjectResult>().Subject;
            ok.Should().NotBeNull();
            ok.Value.Should().Be(Cart);
        }

        [Fact]
        public async Task AddToCart_ReturnOk_WhencartIsExisit()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            }));

            var dto = new AddToCartDto();

            _cartServiceMock.Setup(x => x.AddToCartAsync(1, dto)).Returns(Task.CompletedTask);

            _cartController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var actual = await _cartController.AddToCart(dto);

            // Assert
            var ok = actual.Should().BeOfType<OkObjectResult>().Subject;
            ok.Should().NotBeNull();
        }

        [Fact]
        public async Task AddToCart_ReturnNotFound_WhenProductIsNotExisit()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            }));

            var dto = new AddToCartDto();

            _cartServiceMock.Setup(x => x.AddToCartAsync(1, dto)).ThrowsAsync(new KeyNotFoundException("Product not found"));

            _cartController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var actual = await _cartController.AddToCart(dto);

            // Assert
            var notFound = actual.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFound.Should().NotBeNull();
        }

        [Fact]
        public async Task AddToCart_ReturnsBadRequest_WhenInvalidOperation()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            }));

            var dto = new AddToCartDto();

            _cartServiceMock.Setup(x => x.AddToCartAsync(1, dto)).ThrowsAsync(new InvalidOperationException("Invalid stock"));

            _cartController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var actual = await _cartController.AddToCart(dto);

            // Assert
            var bad = actual.Should().BeOfType<BadRequestObjectResult>();
            bad.Should().NotBeNull();
        }

        [Fact]
        public async Task ClearCart_ReturnsOk()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            }));

            _cartServiceMock.Setup(x => x.ClearCartAsync(1)).Returns(Task.CompletedTask);

            _cartController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var actual = await _cartController.ClearCart();

            // Assert
            var ok = actual.Should().BeOfType<OkObjectResult>();
            ok.Should().NotBeNull();
        }
    }
}
