namespace ECommerce.Infrastructure.Configurations
{
    public class PaymobOptions
    {
        public const string SectionName = "Paymob";

        public string ApiKey { get; set; } = string.Empty;
        public string HmacSecret { get; set; } = string.Empty;
        public PaymobIntegrationIds IntegrationId { get; set; } = new();
        public PaymobIframeIds IframeId { get; set; } = new();
        public string BaseUrl { get; set; } = "https://accept.paymob.com/api";
        public int PaymentKeyExpirationSeconds { get; set; } = 3600;
        public string Currency { get; set; } = "EGP";
    }

    public class PaymobIntegrationIds
    {
        public int Card { get; set; }
    }

    public class PaymobIframeIds
    {
        public string Card { get; set; } = string.Empty;
    }
}
