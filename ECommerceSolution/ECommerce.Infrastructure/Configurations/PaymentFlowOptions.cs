namespace ECommerce.Infrastructure.Configurations
{
    public class PaymentFlowOptions
    {
        public const string SectionName = "PaymentFlow";

        public bool EnableLocalBypass { get; set; }
    }
}
