using ECommerce.Web.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Text;

namespace ECommerce.Tests.Services
{
    public class ProductImageStoreTests
    {
        private readonly string _tempRoot;
        private readonly string _tempWebRoot;

        public ProductImageStoreTests()
        {
            _tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            _tempWebRoot = Path.Combine(_tempRoot, "wwwroot");

            Directory.CreateDirectory(_tempRoot);
            Directory.CreateDirectory(_tempWebRoot);
        }

        private ProductImageStore CreateStore()
        {
            var envMock = new Mock<IWebHostEnvironment>();

            envMock.Setup(x => x.ContentRootPath).Returns(_tempRoot);
            envMock.Setup(x => x.WebRootPath).Returns(_tempWebRoot);

            return new ProductImageStore(envMock.Object);
        }

        private IFormFile CreateFile()
        {
            var bytes = Encoding.UTF8.GetBytes("fake-image");
            var stream = new MemoryStream(bytes);

            return new FormFile(stream, 0, bytes.Length, "file", "image.jpg")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };
        }

        [Fact]
        public async Task GetImageUrl_ReturnUrl_AfterSavingFile()
        {
            var store = CreateStore();
            var file = CreateFile();

            await store.SaveImageAsync(1, file, null);

            // Act
            var url = store.GetImageUrl(1);

            // Assert
            url.Should().NotBeNull();
            url.Should().StartWith("/uploads/products/");
        }

        [Fact]
        public async Task SaveImageAsync_Return_WhenSave()
        {
            // Arrange
            var store = CreateStore();
            var file = CreateFile();

            // Act
            await store.SaveImageAsync(1, file, null);

            // Assert
            var files = Directory.GetFiles(
                Path.Combine(_tempWebRoot, "uploads", "products")
            );

            files.Should().NotBeEmpty();
            files.Should().ContainSingle(f => f.Contains("product-1-"));
        }

        [Fact]
        public async Task SaveImageAsync_Return_WhenUrlExist()
        {
            // Arrange
            var store = CreateStore();

            // Act
            await store.SaveImageAsync(2, null, " https://img.com/p.png ");

            // Assert
            var url = store.GetImageUrl(2);

            url.Should().Be("https://img.com/p.png");
        }

        [Fact]
        public async Task SaveImageAsync_Return_WhenNull()
        {
            // Arrange
            var store = CreateStore();

            // Act
            await store.SaveImageAsync(3, null, null);

            // Assert
            store.GetImageUrl(3).Should().BeNull();
        }
    }
}