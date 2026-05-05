namespace ECommerce.Application.DTOs.Order
{
    public class CheckoutRequestDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = "Cairo";
        public string Street { get; set; } = string.Empty;
        public string Building { get; set; } = string.Empty;
        public string Apartment { get; set; } = "NA";
        public string Floor { get; set; } = "NA";
        public string Country { get; set; } = "EG";
        public string ReturnUrl { get; set; } = string.Empty;
    }
}
