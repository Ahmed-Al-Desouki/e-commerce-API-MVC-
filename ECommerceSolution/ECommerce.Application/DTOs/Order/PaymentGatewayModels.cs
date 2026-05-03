namespace ECommerce.Application.DTOs.Order
{
    public class PaymentGatewayRequestDto
    {
        public string MerchantOrderId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "EGP";
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string Building { get; set; } = string.Empty;
        public string Apartment { get; set; } = string.Empty;
        public string Floor { get; set; } = string.Empty;
        public string Country { get; set; } = "EG";
        public IReadOnlyCollection<PaymentGatewayItemDto> Items { get; set; } = Array.Empty<PaymentGatewayItemDto>();
    }

    public class PaymentGatewayItemDto
    {
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class PaymentGatewaySessionDto
    {
        public string ProviderOrderId { get; set; } = string.Empty;
        public string PaymentToken { get; set; } = string.Empty;
        public string CheckoutUrl { get; set; } = string.Empty;
    }
}
