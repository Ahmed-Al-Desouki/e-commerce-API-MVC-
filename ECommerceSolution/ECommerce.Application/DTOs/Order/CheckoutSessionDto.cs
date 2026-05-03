namespace ECommerce.Application.DTOs.Order
{
    public class CheckoutSessionDto
    {
        public int OrderId { get; set; }
        public int PaymentId { get; set; }
        public string CheckoutUrl { get; set; } = string.Empty;
        public DateTime? ExpiresAt { get; set; }
        public bool ReusedPendingPayment { get; set; }
    }
}
