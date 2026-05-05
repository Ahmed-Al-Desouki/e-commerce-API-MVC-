namespace ECommerce.Application.Interfaces.Services
{
    public interface IPaymentFlowSettings
    {
        bool EnableLocalBypass { get; }
    }
}
