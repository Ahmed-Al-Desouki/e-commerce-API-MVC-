using ECommerce.Application.DTOs.Product;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Services;
using ECommerce.Domain.Entities;
using FluentAssertions;
using Moq;

namespace ECommerce.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _productRepoMock;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _productRepoMock = new Mock<IProductRepository>();
            _productService = new ProductService(_productRepoMock.Object);
        }

        //Method_ShouldResult_WhenCondition

        [Fact]
        public async Task CreateAsync_ProductDto_WhenProductIsNotNull()
        {
            // Arrange
            var dto = new CreateProductDto 
            { 
                Name = "Laptop", 
                Price = 10, 
                Stock = 10 
            };

            // Act
            var result = await _productService.CreateAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Laptop");
            result.Price.Should().Be(10);
            result.Stock.Should().Be(10);

            _productRepoMock.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Once);
            _productRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}