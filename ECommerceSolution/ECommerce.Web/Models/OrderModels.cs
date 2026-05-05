using System.ComponentModel.DataAnnotations;

namespace ECommerce.Web.Models
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public List<OrderItemViewModel> Items { get; set; } = new();
    }

    public class OrderItemViewModel
    {
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class CheckoutViewModel
    {
        public CartViewModel Cart { get; set; } = new();

        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Last name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;

        [Required]
        public string State { get; set; } = "Cairo";

        [Required]
        public string Street { get; set; } = string.Empty;

        [Required]
        public string Building { get; set; } = string.Empty;

        public string Apartment { get; set; } = "NA";
        public string Floor { get; set; } = "NA";
        public string Country { get; set; } = "EG";
    }

    public class CheckoutSessionViewModel
    {
        public int OrderId { get; set; }
        public int PaymentId { get; set; }
        public string CheckoutUrl { get; set; } = string.Empty;
        public DateTime? ExpiresAt { get; set; }
        public bool ReusedPendingPayment { get; set; }
    }

    public class PaymentResultViewModel
    {
        public int OrderId { get; set; }
        public bool IsSuccess { get; set; }
        public bool IsPending { get; set; }
        public string Message { get; set; } = string.Empty;
        public string OrderStatus { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }

    public class CheckoutRequestApiModel
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

    public class PaymentCallbackApiModel
    {
        public string? Hmac { get; set; }
        public string? MerchantOrderId { get; set; }
        public string? Order { get; set; }
        public string? Id { get; set; }
        public string? AmountCents { get; set; }
        public string? CreatedAt { get; set; }
        public string? Currency { get; set; }
        public string? ErrorOccured { get; set; }
        public string? HasParentTransaction { get; set; }
        public string? IntegrationId { get; set; }
        public string? Is3DSecure { get; set; }
        public string? IsAuth { get; set; }
        public string? IsCapture { get; set; }
        public string? IsRefunded { get; set; }
        public string? IsStandalonePayment { get; set; }
        public string? IsVoided { get; set; }
        public string? Owner { get; set; }
        public string? Pending { get; set; }
        public string? Success { get; set; }
        public string? SourceDataPan { get; set; }
        public string? SourceDataSubType { get; set; }
        public string? SourceDataType { get; set; }
    }
}
