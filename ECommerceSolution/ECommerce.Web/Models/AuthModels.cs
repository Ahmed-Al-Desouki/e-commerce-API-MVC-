using System.ComponentModel.DataAnnotations;

namespace ECommerce.Web.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public string? ReturnUrl { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, ErrorMessage = "Username must be 50 characters or fewer.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponseModel
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
    }
}
