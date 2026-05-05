using ECommerce.Application.Interfaces.Services;

namespace ECommerce.Infrastructure.Services
{
    public class MerchantOrderIdGenerator : IMerchantOrderIdGenerator
    {
        public string Generate() => $"ECOM-{Guid.NewGuid():N}";
    }
}
