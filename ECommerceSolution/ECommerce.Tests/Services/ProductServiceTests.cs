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


        [Fact]
        public async Task DeleteAsync_KeyNotFoundException_WhenProductIsNull()
        {
            // Arrange
            int id = 1;

            _productRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Product?)null);

            // Act
            var actual = async () => await _productService.DeleteAsync(1);

            // Assert
            await actual.Should().ThrowAsync<KeyNotFoundException>();

            _productRepoMock.Verify(r => r.DeleteAsync(It.IsAny<Product>()), Times.Never);
            _productRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WhenProductIsNotNull()
        {
            // Arrange
            int id = 1;

            var product = new Product
            {
                Name = "Laptop",
                Price = 10,
                Stock = 10
            };

            _productRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(product);

            // Act
            await _productService.DeleteAsync(id);

            // Assert
            _productRepoMock.Verify(i => i.DeleteAsync(product), Times.Once);
            _productRepoMock.Verify(i => i.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ReturnListFromProductsDto()
        {
            // Arrange
            var products = new List<Product>() 
            {
                new Product
                {
                    Id = 1,
                    Name = "Laptop1",
                    Price = 101,
                    Stock = 101
                },
                new Product
                {
                    Id = 2,
                    Name = "Laptop2",
                    Price = 102,
                    Stock = 102
                },
            };

            var productsDto = new List<ProductDto>()
            {
                new ProductDto
                {
                    Id = 1,
                    Name = "Laptop1",
                    Price = 101,
                    Stock = 101
                },
                new ProductDto
                {
                    Id = 2,
                    Name = "Laptop2",
                    Price = 102,
                    Stock = 102
                },
            };

            _productRepoMock.Setup(i => i.GetAllAsync()).ReturnsAsync(products);

            // Act
            var actual = await _productService.GetAllAsync();

            // Assert
            actual.Should().BeEquivalentTo(productsDto);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnProductsDto_WhenProductIsNotNull()
        {
            int id = 1;
            // Arrange
            var product = new Product
            {
                Id = 1,
                Name = "Laptop1",
                Price = 101,
                Stock = 101
            };
            

            var productDto = new ProductDto
            {
                Id = 1,
                Name = "Laptop1",
                Price = 101,
                Stock = 101
            };

            _productRepoMock.Setup(i => i.GetByIdAsync(id)).ReturnsAsync(product);

            // Act
            var actual = await _productService.GetByIdAsync(id);

            // Assert
            actual.Should().BeEquivalentTo(productDto);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnNull_WhenProductIsNull()
        {
            int id = 1;
            // Arrange

            _productRepoMock.Setup(i => i.GetByIdAsync(id)).ReturnsAsync((Product?)null);

            // Act
            var actual = await _productService.GetByIdAsync(id);

            // Assert
            actual.Should().BeNull();
        }
    }
}