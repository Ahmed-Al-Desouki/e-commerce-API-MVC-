namespace ECommerce.Application.DTOs.Order
{
    public class PaymentCallbackDto
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
