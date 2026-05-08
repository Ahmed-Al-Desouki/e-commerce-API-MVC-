using ECommerce.API.Controllers;
using ECommerce.Application.DTOs.Product;
using ECommerce.Application.Interfaces.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ECommerce.Tests.Controllers
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductService> _productServiceMock;
        private readonly ProductsController _productsController;

        public ProductsControllerTests()
        {
            _productServiceMock = new Mock<IProductService>();
            _productsController = new ProductsController(_productServiceMock.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            // Arrange
            var products = new List<ProductDto>();

            _productServiceMock.Setup(x => x.GetAllAsync()).ReturnsAsync(products);

            // Act
            var actual = await _productsController.GetAll();

            // Assert
            var ok = actual.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(products);
        }

        [Fact]
        public async Task GetById_ReturnOk_WhenProductExists()
        {
            // Arrange
            var product = new ProductDto();

            _productServiceMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(product);

            // Act
            var actual = await _productsController.GetById(1);

            // Assert
            var ok = actual.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(product);
        }

        [Fact]
        public async Task GetById_ReturnNotFound_WhenProductIsNotExist()
        {
            // Arrange
            _productServiceMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((ProductDto?)null);

            // Act
            var actual = await _productsController.GetById(1);

            // Assert
            var notFound = actual.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFound.Value.Should().NotBeNull();
        }

        [Fact]
        public async Task Create_ReturnCreatedAtAction()
        {
            // Arrange
            var dto = new CreateProductDto();

            var productDto = new ProductDto { Id = 10 };

            _productServiceMock.Setup(x => x.CreateAsync(dto)).ReturnsAsync(productDto);

            // Act
            var actual = await _productsController.Create(dto);

            // Assert
            var created = actual.Should().BeOfType<CreatedAtActionResult>().Subject;
            created.Value.Should().Be(productDto);
            created.ActionName.Should().Be(nameof(_productsController.GetById));
        }

        [Fact]
        public async Task Update_ReturnOk_WhenProductUpdated()
        {
            // Arrange
            var updateProductDto = new UpdateProductDto();

            var productDto = new ProductDto();

            _productServiceMock.Setup(x => x.UpdateAsync(1, updateProductDto)).ReturnsAsync(productDto);

            // Act
            var actual = await _productsController.Update(1, updateProductDto);

            // Assert
            var ok = actual.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().Be(productDto);
        }


        [Fact]
        public async Task Update_ReturnNotFound_WhenProductNotFound()
        {
            // Arrange
            var dto = new UpdateProductDto();

            _productServiceMock.Setup(x => x.UpdateAsync(1, dto)).ThrowsAsync(new KeyNotFoundException("Product not found"));

            // Act
            var actual = await _productsController.Update(1, dto);

            // Assert
            var notFound = actual.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFound.Value.Should().NotBeNull();
        }

        [Fact]
        public async Task Delete_ReturnNoContent()
        {
            // Arrange
            _productServiceMock.Setup(x => x.DeleteAsync(1)).Returns(Task.CompletedTask);

            // Act
            var actual = await _productsController.Delete(1);

            // Assert
            actual.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_ReturnNotFound_WhenProductNotFound()
        {
            // Arrange
            _productServiceMock.Setup(x => x.DeleteAsync(1)).ThrowsAsync(new KeyNotFoundException("Product not found"));

            // Act
            var actual = await _productsController.Delete(1);

            // Assert
            var notFound = actual.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFound.Value.Should().NotBeNull();
        }


    }
}