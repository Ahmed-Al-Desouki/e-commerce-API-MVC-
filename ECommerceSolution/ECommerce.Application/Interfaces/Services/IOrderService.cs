using ECommerce.Application.DTOs.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetUserOrdersAsync(int userId);
        Task<CheckoutSessionDto> InitiateCheckoutAsync(int userId, CheckoutRequestDto request, CancellationToken cancellationToken = default);
        Task<PaymentResultDto> ProcessPaymentCallbackAsync(PaymentCallbackDto callback, CancellationToken cancellationToken = default);
    }
}
