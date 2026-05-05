namespace ECommerce.Application.DTOs.Order
{
    public class PaymentResultDto
    {
        public int OrderId { get; set; }
        public bool IsSuccess { get; set; }
        public bool IsPending { get; set; }
        public string Message { get; set; } = string.Empty;
        public string OrderStatus { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }
}
