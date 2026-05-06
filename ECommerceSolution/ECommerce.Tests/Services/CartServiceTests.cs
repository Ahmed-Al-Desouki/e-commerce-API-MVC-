using ECommerce.Application.DTOs.Cart;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Services;
using ECommerce.Domain.Entities;
using FluentAssertions;
using Moq;

namespace ECommerce.Tests.Services
{
    public class CartServiceTests
    {
        private readonly Mock<ICartRepository> _cartRepoMock;
        private readonly Mock<IProductRepository> _productRepoMock;
        private readonly CartService _cartService;

        public CartServiceTests()
        {
            _cartRepoMock = new Mock<ICartRepository>();
            _productRepoMock = new Mock<IProductRepository>();
            _cartService = new CartService(_cartRepoMock.Object, _productRepoMock.Object);
        }

        [Fact]
        public async Task GetCartAsync_ReturnNewObjectCart_WhenCartIsNull()
        {
            // Arrange
            int userId = 1;

            _cartRepoMock.Setup(i => i.GetByUserIdAsync(userId)).ReturnsAsync((Cart?)null);

            // Act
            var actual = await _cartService.GetCartAsync(userId);

            // Assert
            actual.Should().BeEquivalentTo(new CartDto());
        }

        [Fact]
        public async Task GetCartAsync_ReturnCartDto_WhenCartIsNotNull()
        {
            // Arrange
            int userId = 1;

            var cart = new Cart
            {
                Id = 1,
                UserId = 1,
                Items = new List<CartItem>
                {
                    new CartItem
                    {
                        ProductId = 1,
                        Quantity = 101,
                        Product = new Product
                        {
                            Id = 1,
                            Name = "Laptop",
                            Price = 101
                        }
                    },
                    new CartItem
                    {
                        ProductId = 2,
                        Quantity = 102,
                        Product = new Product
                        {
                            Id = 2,
                            Name = "Laptop2",
                            Price = 102
                        }
                    }
                }
            };

            var cartDto = new CartDto
            {
                CartId = 1,
                Items = new List<CartItemDto>()
                {
                    new CartItemDto
                    {
                        ProductId = 1,
                        ProductName = "Laptop",
                        UnitPrice = 101,
                        Quantity = 101,
                    },
                    new CartItemDto
                    {
                        ProductId = 2,
                        ProductName = "Laptop2",
                        UnitPrice = 102,
                        Quantity = 102,
                    },
                },
            };

            _cartRepoMock.Setup(i => i.GetByUserIdAsync(userId)).ReturnsAsync(cart);

            // Act
            var actual = await _cartService.GetCartAsync(userId);

            // Assert
            actual.Should().BeEquivalentTo(cartDto);
        }


        [Fact]
        public async Task AddToCartAsync_KeyNotFoundException_WhenProductIsNull()
        {
            // Arrange
            int userId = 1;
            var dto = new AddToCartDto
            {
                ProductId = 1,
                Quantity = 2
            };

            _productRepoMock.Setup(i => i.GetByIdAsync(dto.ProductId)).ReturnsAsync((Product?)null);

            // Act
            var act = async () => await _cartService.AddToCartAsync(userId, dto);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact]
        public async Task AddToCartAsync_InvalidOperationException_WhenStockIsLessThantQuantity()
        {
            // Arrange
            int userId = 1;
            var dto = new AddToCartDto
            {
                ProductId = 1,
                Quantity = 10
            };

            var product = new Product
            {
                Id = 1,
                Name = "Laptop",
                Stock = 5
            };

            _productRepoMock.Setup(i => i.GetByIdAsync(dto.ProductId)).ReturnsAsync(product);

            // Act
            var act = async () => await _cartService.AddToCartAsync(userId, dto);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task AddToCartAsync_ReturnCreateCart_WhenCartIsNull()
        {
            // Arrange
            int userId = 1;
            var dto = new AddToCartDto
            {
                ProductId = 1,
                Quantity = 2
            };

            var product = new Product
            {
                Id = 1,
                Name = "Laptop",
                Stock = 10
            };


            _productRepoMock.Setup(i => i.GetByIdAsync(dto.ProductId)).ReturnsAsync(product);

            _cartRepoMock.Setup(i => i.GetByUserIdAsync(userId)).ReturnsAsync(new Cart { UserId = userId });

            // Act
            await _cartService.AddToCartAsync(userId, dto);

            // Assert
            _cartRepoMock.Verify(i => i.AddAsync(It.IsAny<Cart>()), Times.Once);

            _cartRepoMock.Verify(i => i.SaveChangesAsync(), Times.Once);
        }


        [Fact]
        public async Task AddToCartAsync_AddCartItemInCart_WhenExistingItemIsNull()
        {
            // Arrange
            int userId = 1;

            var dto = new AddToCartDto
            {
                ProductId = 1,
                Quantity = 2
            };

            var product = new Product
            {
                Id = 1,
                Name = "Laptop",
                Stock = 10
            };

            var cart = new Cart
            {
                Id = 1,
                UserId = userId,
                Items = new List<CartItem>() 
            };

            _productRepoMock.Setup(i => i.GetByIdAsync(dto.ProductId)).ReturnsAsync(product);
            _cartRepoMock.Setup(i => i.GetByUserIdAsync(userId)).ReturnsAsync(cart);

            // Act
            await _cartService.AddToCartAsync(userId, dto);

            // Assert
            cart.Items.First().ProductId.Should().Be(dto.ProductId);
            cart.Items.First().Quantity.Should().Be(dto.Quantity);

            _cartRepoMock.Verify(i => i.SaveChangesAsync(), Times.Once);
        }

        [Theory]
        [InlineData(2, 3, 5)]
        [InlineData(1, 4, 5)]
        [InlineData(10, 5, 15)]
        public async Task AddToCartAsync_SumQuantity_WhenExistingItemIsNotNull(int q1,int q2,int total)
        {
            // Arrange
            int userId = 1;

            var dto = new AddToCartDto
            {
                ProductId = 1,
                Quantity = q1
            };

            var product = new Product
            {
                Id = 1,
                Name = "Laptop",
                Stock = 100
            };

            var cart = new Cart
            {
                Id = 1,
                UserId = userId,
                Items = new List<CartItem>
                {
                    new CartItem
                    {
                        ProductId = 1,
                        Quantity = q2
                    }
                }
            };

            _productRepoMock.Setup(i => i.GetByIdAsync(dto.ProductId)) .ReturnsAsync(product);

            _cartRepoMock.Setup(i => i.GetByUserIdAsync(userId)).ReturnsAsync(cart);

            // Act
            await _cartService.AddToCartAsync(userId, dto);

            // Assert
            cart.Items.First().Quantity.Should().Be(total);

            _cartRepoMock.Verify(i => i.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ClearCartAsync_Return_WhenCartIsNull()
        {
            // Arrange
            int userId = 1;

            _cartRepoMock.Setup(i => i.GetByUserIdAsync(userId)).ReturnsAsync((Cart?)null);

            // Act
            await _cartService.ClearCartAsync(userId);

            // Assert
            _cartRepoMock.Verify(i => i.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task ClearCartAsync_ClearItems_WhenCartIsNotNull()
        {
            // Arrange
            int userId = 1;

            var cart = new Cart
            {
                Id = 1,
                UserId = userId,
                Items = new List<CartItem>
                {
                    new CartItem 
                    {
                        ProductId = 1, 
                        Quantity = 2 
                    },
                    new CartItem {
                        ProductId = 2, 
                        Quantity = 3 
                    }
                }
            };

            _cartRepoMock.Setup(c => c.GetByUserIdAsync(userId)).ReturnsAsync(cart);

            // Act
            await _cartService.ClearCartAsync(userId);

            // Assert
            cart.Items.Should().BeEmpty();

            _cartRepoMock.Verify(c => c.SaveChangesAsync(), Times.Once);
        }
    }
}
