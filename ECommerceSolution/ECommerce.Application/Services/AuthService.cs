using ECommerce.Application.DTOs.Auth;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IPasswordHasher _passwordHasher;

        public AuthService(
            IUserRepository userRepository,
            IJwtService jwtService,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _passwordHasher = passwordHasher;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto)
        {
            ValidateRegisterRequest(dto);

            var emailExists = await _userRepository.EmailExistsAsync(dto.Email);
            if (emailExists)
                throw new InvalidOperationException("Email is already registered.");

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                Password = _passwordHasher.HashPassword(dto.Password)
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            var token = _jwtService.GenerateToken(user);

            return new AuthResponseDto
            {
                UserId = user.Id,
                Username = user.Username,
                Token = token,
                IsAdmin = user.IsAdmin
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
        {
            ValidateLoginRequest(dto);

            var user = await _userRepository.GetByEmailAsync(dto.Email);

            if (user is null || !_passwordHasher.VerifyPassword(dto.Password, user.Password))

            if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password)) //make class for testing
                throw new UnauthorizedAccessException("Invalid email or password.");

            var token = _jwtService.GenerateToken(user);

            return new AuthResponseDto
            {
                UserId = user.Id,
                Username = user.Username,
                Token = token,
                IsAdmin = user.IsAdmin
            };
        }

        private static void ValidateRegisterRequest(RegisterRequestDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            if (string.IsNullOrWhiteSpace(dto.Username))
                throw new ArgumentException("Username is required.", nameof(dto));

            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ArgumentException("Email is required.", nameof(dto));

            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new ArgumentException("Password is required.", nameof(dto));
        }

        private static void ValidateLoginRequest(LoginRequestDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ArgumentException("Email is required.", nameof(dto));

            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new ArgumentException("Password is required.", nameof(dto));
        }
    }
}
