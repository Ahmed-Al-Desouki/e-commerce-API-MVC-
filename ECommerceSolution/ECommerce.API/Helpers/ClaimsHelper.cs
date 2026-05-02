using System.Security.Claims;

namespace ECommerce.API.Helpers
{
    /// <summary>
    /// A small helper that reads the user ID out of the JWT claims. Used by every protected controller — keeps controllers clean.
    /// </summary>
    public static class ClaimsHelper
    {
        public static int GetUserId(ClaimsPrincipal user)
        {
            var value = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(value) || !int.TryParse(value, out var userId))
                throw new UnauthorizedAccessException("User identity is invalid.");

            return userId;
        }
    }
}
