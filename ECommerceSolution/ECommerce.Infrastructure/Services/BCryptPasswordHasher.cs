using ECommerce.Application.Interfaces.Services;

namespace ECommerce.Infrastructure.Services
{
    public class BCryptPasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password) =>
            BCrypt.Net.BCrypt.HashPassword(password);

        public bool VerifyPassword(string password, string hashedPassword) =>
            BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}
