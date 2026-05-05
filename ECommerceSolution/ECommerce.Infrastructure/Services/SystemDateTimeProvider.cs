using ECommerce.Application.Interfaces.Services;

namespace ECommerce.Infrastructure.Services
{
    public class SystemDateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
