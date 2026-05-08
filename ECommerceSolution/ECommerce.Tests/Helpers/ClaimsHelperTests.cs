using ECommerce.API.Helpers;
using FluentAssertions;
using System.Security.Claims;


namespace ECommerce.Tests.Helpers
{
    public class ClaimsHelperTests
    {
        [Fact]
        public void GetUserId_ReturnsUserId_WhenClaimIsCorrect()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "10")
            }));

            // Act
            var actual = ClaimsHelper.GetUserId(user);

            // Assert
            actual.Should().Be(10);
        }

        [Fact]
        public void GetUserId_UnauthorizedAccessException_WhenClaimIsNotExist()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity());

            // Act
            var actual = () => ClaimsHelper.GetUserId(user);

            // Assert
            actual.Should().Throw<UnauthorizedAccessException>();
        }
    }
}
