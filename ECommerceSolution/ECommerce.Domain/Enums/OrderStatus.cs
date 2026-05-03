namespace ECommerce.Domain.Enums
{
    public enum OrderStatus
    {
        PendingPayment = 1,
        Paid = 2,
        PaymentFailed = 3,
        PaymentExpired = 4
    }
}
