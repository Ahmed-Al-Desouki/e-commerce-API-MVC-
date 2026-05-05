using ECommerce.Application.DTOs.Auth;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Application.Services;
using ECommerce.Domain.Entities;
using FluentAssertions;
using Moq;

namespace ECommerce.Tests.Services
{
    public class AuthServiceTests
    {
        //Method_ShouldResult_WhenCondition

        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IJwtService> _jwtServiceMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _jwtServiceMock = new Mock<IJwtService>();

            _authService = new AuthService(_userRepoMock.Object, _jwtServiceMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_InvalidOperationException_WhenEmailExistsIsTrue()
        {
            // arange
            var dto = new RegisterRequestDto
            {
                Email = "amohamedahmad68@gmail.com"
            };

            _userRepoMock.Setup(i => i.EmailExistsAsync(dto.Email)).ReturnsAsync(true);

            // act
            var actual = async () => await _authService.RegisterAsync(dto);


            // assert
            await actual.Should().ThrowAsync<InvalidOperationException>();

            _userRepoMock.Verify(i => i.AddAsync(It.IsAny<User>()), Times.Never);
            _userRepoMock.Verify(i => i.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task RegisterAsync_CreateUser_WhenEmailExistsIsFalse()
        {
            // arange
            var dto = new RegisterRequestDto
            {
                Username = "Mohamed",
                Email = "amohamedahmad68@gmail.com",
                Password = "Moh1239987"
            };

            _userRepoMock.Setup(i => i.EmailExistsAsync(dto.Email)).ReturnsAsync(false);
            _jwtServiceMock.Setup(i => i.GenerateToken(It.IsAny<User>())).Returns("token123212");

            // act
            var actual = await _authService.RegisterAsync(dto);

            // assert
            actual.Should().NotBeNull();
            actual.Token.Should().Be("token123212");
            actual.Username.Should().Be(dto.Username);
            actual.IsAdmin.Should().Be(false);

            _userRepoMock.Verify(i => i.AddAsync(It.IsAny<User>()), Times.Once);
            _userRepoMock.Verify(i => i.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_UnauthorizedAccessException_WhenUserIsNull()
        {
            // arange
            var dto = new LoginRequestDto
            {
                Email = "amohamed@gmail.com",
                Password = "Moh165656"
            };

            _userRepoMock.Setup(i => i.GetByEmailAsync(dto.Email)).ReturnsAsync((User?)null);

            // act
            var actual = async () => await _authService.LoginAsync(dto);

            // assert
            await actual.Should().ThrowAsync<UnauthorizedAccessException>();

            _userRepoMock.Verify(i => i.AddAsync(It.IsAny<User>()), Times.Never);
            _userRepoMock.Verify(i => i.SaveChangesAsync(), Times.Never);
        }


        //[Theory]
        //[InlineData(true)]
        //[InlineData(false)]
        //public async Task LoginAsync_LoginUser_WhenUserIsNotNull(bool isAdmin)
        //{
        //    // arange
        //    var dto = new LoginRequestDto
        //    {
        //        Email = "amohamed@gmail.com",
        //        Password = "Moh165656"
        //    };

        //    var user = new User
        //    {
        //        Id = 1,
        //        Email = dto.Email,
        //        Username = "Mohamed",
        //        Password = "$#43434v3fh3l4fc4f",
        //        IsAdmin = isAdmin
        //    };

        //    _userRepoMock.Setup(i => i.GetByEmailAsync(dto.Email)).ReturnsAsync(user);
        //    _jwtServiceMock.Setup(i => i.GenerateToken(It.IsAny<User>())).Returns("Token65444455");

        //    // act
        //    var actual = await _authService.LoginAsync(dto);

        //    // assert
        //    actual.Should().NotBeNull();
        //    actual.Token.Should().Be("Token65444455");
        //    actual.IsAdmin.Should().Be(isAdmin);
        //}
    }
}
