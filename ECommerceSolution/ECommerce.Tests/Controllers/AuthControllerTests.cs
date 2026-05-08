using ECommerce.API.Controllers;
using ECommerce.Application.DTOs.Auth;
using ECommerce.Application.Interfaces.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ECommerce.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly AuthController _authController;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _authController = new AuthController(_authServiceMock.Object);
        }

        [Fact]
        public async Task Register_ReturnOk_WhenRegisterIsOk()
        {
            // Arrange
            var dto = new RegisterRequestDto
            {
                Username = "Mohamed",
                Email = "amohamed.com",
                Password = "123456"
            };

            var authResponseDto = new AuthResponseDto
            {
                Token = "tttyht565"
            };

            _authServiceMock.Setup(x => x.RegisterAsync(dto)).ReturnsAsync(authResponseDto);

            // Act
            var actual = await _authController.Register(dto);

            // Assert
            var ok = actual.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(authResponseDto);
        }

        [Fact]
        public async Task Register_ReturnConflict_WhenUserAlreadyExists()
        {
            // Arrange
            var dto = new RegisterRequestDto
            {
                Username = "Mohamed",
                Email = "amohamed.com",
                Password = "123456"
            };

            _authServiceMock.Setup(x => x.RegisterAsync(dto)).ThrowsAsync(new InvalidOperationException("User already exists"));

            // Act
            var actual = await _authController.Register(dto);

            // Assert
            var error = actual.Should().BeOfType<ConflictObjectResult>().Subject;

            error.Value.Should().BeEquivalentTo(new
            {
                message = "User already exists"
            });
        }

        [Fact]
        public async Task Login_ReturnOk_WhenCredentialsAreValid()
        {
            // Arrange
            var dto = new LoginRequestDto
            {
                Email = "amohamed.com",
                Password = "123456"
            };

            var authResponseDto = new AuthResponseDto
            {
                Token = "uitui545454"
            };

            _authServiceMock.Setup(x => x.LoginAsync(dto)).ReturnsAsync(authResponseDto);

            // Act
            var actual = await _authController.Login(dto);

            // Assert
            var ok = actual.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(authResponseDto);
        }

        [Fact]
        public async Task Login_UnauthorizedObjectResult_WhenCredentialsAreInvalid()
        {
            // Arrange
            var dto = new LoginRequestDto
            {
                Email = "test@test.com",
                Password = "wrong-password"
            };

            _authServiceMock.Setup(x => x.LoginAsync(dto)).ThrowsAsync(new UnauthorizedAccessException("Invalid credentials"));

            // Act
            var result = await _authController.Login(dto);

            // Assert
            var error = result.Should().BeOfType<UnauthorizedObjectResult>().Subject;

            error.Value.Should().BeEquivalentTo(new
            {
                message = "Invalid credentials"
            });
        }
    }
}
