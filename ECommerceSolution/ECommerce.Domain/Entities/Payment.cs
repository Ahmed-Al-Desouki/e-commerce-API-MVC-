using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string Provider { get; set; } = string.Empty;
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "EGP";
        public string MerchantOrderId { get; set; } = string.Empty;
        public string? ProviderOrderId { get; set; }
        public string? ProviderTransactionId { get; set; }
        public string? PaymentToken { get; set; }
        public string? CheckoutUrl { get; set; }
        public string? CardLastFour { get; set; }
        public string? CardType { get; set; }
        public string? FailureReason { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public Order Order { get; set; } = null!;
    }
}
