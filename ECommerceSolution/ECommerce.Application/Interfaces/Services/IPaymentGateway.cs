using ECommerce.Application.DTOs.Order;

namespace ECommerce.Application.Interfaces.Services
{
    public interface IPaymentGateway
    {
        Task<PaymentGatewaySessionDto> CreateCardPaymentAsync(PaymentGatewayRequestDto request, CancellationToken cancellationToken = default);
        bool IsCallbackAuthentic(PaymentCallbackDto callback);
    }
}
