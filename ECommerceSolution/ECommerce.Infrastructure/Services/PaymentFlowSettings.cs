using ECommerce.Application.Interfaces.Services;
using ECommerce.Infrastructure.Configurations;
using Microsoft.Extensions.Options;

namespace ECommerce.Infrastructure.Services
{
    public class PaymentFlowSettings : IPaymentFlowSettings
    {
        private readonly IOptions<PaymentFlowOptions> _options;

        public PaymentFlowSettings(IOptions<PaymentFlowOptions> options)
        {
            _options = options;
        }

        public bool EnableLocalBypass => _options.Value.EnableLocalBypass;
    }
}
