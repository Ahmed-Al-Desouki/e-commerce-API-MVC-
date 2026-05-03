using ECommerce.Application.DTOs.Order;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;

namespace ECommerce.Application.Services
{
    public class OrderService : IOrderService
    {
        private static readonly TimeSpan PendingPaymentWindow = TimeSpan.FromHours(1);

        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPaymentGateway _paymentGateway;

        public OrderService(
            IOrderRepository orderRepository,
            ICartRepository cartRepository,
            IProductRepository productRepository,
            IUserRepository userRepository,
            IPaymentGateway paymentGateway)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _paymentGateway = paymentGateway;
        }

        public async Task<IEnumerable<OrderDto>> GetUserOrdersAsync(int userId)
        {
            await ReleaseExpiredPendingOrdersAsync(userId);
            var orders = await _orderRepository.GetByUserIdAsync(userId);
            return orders.Select(MapToDto);
        }

        public async Task<CheckoutSessionDto> InitiateCheckoutAsync(int userId, CheckoutRequestDto request, CancellationToken cancellationToken = default)
        {
            ValidateCheckoutRequest(request);
            await ReleaseExpiredPendingOrdersAsync(userId);

            var existingPendingOrder = (await _orderRepository.GetByUserIdAsync(userId))
                .FirstOrDefault(order =>
                    order.Status == OrderStatus.PendingPayment &&
                    order.Payment is not null &&
                    order.Payment.Status == PaymentStatus.Pending &&
                    !string.IsNullOrWhiteSpace(order.Payment.CheckoutUrl) &&
                    order.Payment.ExpiresAt > DateTime.UtcNow);

            if (existingPendingOrder?.Payment is not null)
            {
                return new CheckoutSessionDto
                {
                    OrderId = existingPendingOrder.Id,
                    PaymentId = existingPendingOrder.Payment.Id,
                    CheckoutUrl = existingPendingOrder.Payment.CheckoutUrl!,
                    ExpiresAt = existingPendingOrder.Payment.ExpiresAt,
                    ReusedPendingPayment = true
                };
            }

            var cart = await _cartRepository.GetByUserIdAsync(userId);
            if (cart is null || !cart.Items.Any())
            {
                throw new InvalidOperationException("Cart is empty.");
            }

            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new InvalidOperationException("User was not found.");

            var orderItems = new List<OrderItem>();

            foreach (var cartItem in cart.Items)
            {
                var product = await _productRepository.GetByIdAsync(cartItem.ProductId);
                if (product is null)
                {
                    continue;
                }

                if (product.Stock < cartItem.Quantity)
                {
                    throw new InvalidOperationException($"Insufficient stock for {product.Name}.");
                }

                product.Stock -= cartItem.Quantity;

                orderItems.Add(new OrderItem
                {
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    Price = product.Price
                });
            }

            if (!orderItems.Any())
            {
                throw new InvalidOperationException("Cart is empty.");
            }

            var merchantOrderId = $"ECOM-{Guid.NewGuid():N}";
            var order = new Order
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                TotalAmount = orderItems.Sum(item => item.Price * item.Quantity),
                Status = OrderStatus.PendingPayment,
                BillingFirstName = request.FirstName,
                BillingLastName = request.LastName,
                BillingEmail = string.IsNullOrWhiteSpace(request.Email) ? user.Email : request.Email,
                BillingPhoneNumber = request.PhoneNumber,
                BillingStreet = request.Street,
                BillingBuilding = request.Building,
                BillingCity = request.City,
                BillingCountry = request.Country,
                BillingState = request.State,
                BillingApartment = request.Apartment,
                BillingFloor = request.Floor,
                Items = orderItems,
                Payment = new Payment
                {
                    Provider = "Paymob",
                    Status = PaymentStatus.Pending,
                    Amount = orderItems.Sum(item => item.Price * item.Quantity),
                    Currency = "EGP",
                    MerchantOrderId = merchantOrderId,
                    ExpiresAt = DateTime.UtcNow.Add(PendingPaymentWindow)
                }
            };

            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveChangesAsync();

            try
            {
                var paymentSession = await _paymentGateway.CreateCardPaymentAsync(new PaymentGatewayRequestDto
                {
                    MerchantOrderId = merchantOrderId,
                    Amount = order.TotalAmount,
                    Currency = order.Payment!.Currency,
                    FirstName = order.BillingFirstName,
                    LastName = order.BillingLastName,
                    Email = order.BillingEmail,
                    PhoneNumber = order.BillingPhoneNumber,
                    City = order.BillingCity,
                    State = order.BillingState,
                    Street = order.BillingStreet,
                    Building = order.BillingBuilding,
                    Apartment = order.BillingApartment,
                    Floor = order.BillingFloor,
                    Country = order.BillingCountry,
                    Items = order.Items.Select(item => new PaymentGatewayItemDto
                    {
                        Name = item.Product?.Name ?? $"Product {item.ProductId}",
                        Quantity = item.Quantity,
                        UnitPrice = item.Price
                    }).ToList()
                }, cancellationToken);

                order.Payment.ProviderOrderId = paymentSession.ProviderOrderId;
                order.Payment.PaymentToken = paymentSession.PaymentToken;
                order.Payment.CheckoutUrl = paymentSession.CheckoutUrl;

                await _orderRepository.SaveChangesAsync();

                return new CheckoutSessionDto
                {
                    OrderId = order.Id,
                    PaymentId = order.Payment.Id,
                    CheckoutUrl = paymentSession.CheckoutUrl,
                    ExpiresAt = order.Payment.ExpiresAt
                };
            }
            catch
            {
                FailOrderAndRestoreStock(order, "Unable to create the Paymob payment session.");
                await _orderRepository.SaveChangesAsync();
                throw;
            }
        }

        public async Task<PaymentResultDto> ProcessPaymentCallbackAsync(PaymentCallbackDto callback, CancellationToken cancellationToken = default)
        {
            _ = cancellationToken;

            if (!_paymentGateway.IsCallbackAuthentic(callback))
            {
                throw new InvalidOperationException("Invalid Paymob callback signature.");
            }

            if (string.IsNullOrWhiteSpace(callback.MerchantOrderId))
            {
                throw new InvalidOperationException("Merchant order id is missing from Paymob callback.");
            }

            var order = await _orderRepository.GetByMerchantOrderIdAsync(callback.MerchantOrderId)
                ?? throw new InvalidOperationException("Order linked to the payment callback was not found.");

            if (order.Payment is null)
            {
                throw new InvalidOperationException("Payment record linked to the order was not found.");
            }

            if (order.Status == OrderStatus.Paid && order.Payment.Status == PaymentStatus.Succeeded)
            {
                return CreatePaymentResult(order, true, false, "Payment was already confirmed.");
            }

            order.Payment.ProviderTransactionId = callback.Id;
            order.Payment.ProviderOrderId = callback.Order ?? order.Payment.ProviderOrderId;
            order.Payment.CardLastFour = ExtractLastFour(callback.SourceDataPan);
            order.Payment.CardType = callback.SourceDataSubType ?? callback.SourceDataType;

            var isPending = IsTrue(callback.Pending);
            var isSuccess = IsTrue(callback.Success)
                && !IsTrue(callback.ErrorOccured)
                && !IsTrue(callback.IsVoided)
                && !IsTrue(callback.IsRefunded)
                && !isPending;

            if (isSuccess)
            {
                order.Status = OrderStatus.Paid;
                order.Payment.Status = PaymentStatus.Succeeded;
                order.Payment.CompletedAt = DateTime.UtcNow;

                var cart = await _cartRepository.GetByUserIdAsync(order.UserId);
                if (cart is not null)
                {
                    ClearPurchasedItemsFromCart(cart, order);
                }

                await _orderRepository.SaveChangesAsync();
                return CreatePaymentResult(order, true, false, "Payment completed successfully.");
            }

            if (isPending)
            {
                await _orderRepository.SaveChangesAsync();
                return CreatePaymentResult(order, false, true, "Payment is still pending. Please wait a moment and refresh.");
            }

            FailOrderAndRestoreStock(order, "Payment was declined or cancelled.");
            await _orderRepository.SaveChangesAsync();
            return CreatePaymentResult(order, false, false, "Payment failed or was cancelled.");
        }

        private async Task ReleaseExpiredPendingOrdersAsync(int userId)
        {
            var orders = await _orderRepository.GetByUserIdAsync(userId);
            var expiredOrders = orders
                .Where(order =>
                    order.Status == OrderStatus.PendingPayment &&
                    order.Payment is not null &&
                    order.Payment.Status == PaymentStatus.Pending &&
                    order.Payment.ExpiresAt <= DateTime.UtcNow)
                .ToList();

            if (!expiredOrders.Any())
            {
                return;
            }

            foreach (var order in expiredOrders)
            {
                ExpireOrderAndRestoreStock(order);
            }

            await _orderRepository.SaveChangesAsync();
        }

        private static OrderDto MapToDto(Order order) => new()
        {
            OrderId = order.Id,
            CreatedAt = order.CreatedAt,
            TotalAmount = order.TotalAmount,
            OrderStatus = order.Status.ToString(),
            PaymentStatus = order.Payment?.Status.ToString() ?? "Unavailable",
            Items = order.Items.Select(item => new OrderItemDto
            {
                ProductName = item.Product?.Name ?? string.Empty,
                Quantity = item.Quantity,
                Price = item.Price
            }).ToList()
        };

        private static void ValidateCheckoutRequest(CheckoutRequestDto request)
        {
            var fields = new Dictionary<string, string?>
            {
                ["First name"] = request.FirstName,
                ["Last name"] = request.LastName,
                ["Email"] = request.Email,
                ["Phone number"] = request.PhoneNumber,
                ["City"] = request.City,
                ["State"] = request.State,
                ["Street"] = request.Street,
                ["Building"] = request.Building,
                ["Country"] = request.Country
            };

            var missingField = fields.FirstOrDefault(field => string.IsNullOrWhiteSpace(field.Value));
            if (!string.IsNullOrWhiteSpace(missingField.Key))
            {
                throw new InvalidOperationException($"{missingField.Key} is required to start payment.");
            }
        }

        private static void FailOrderAndRestoreStock(Order order, string reason)
        {
            if (order.Status != OrderStatus.PendingPayment || order.Payment is null)
            {
                return;
            }

            foreach (var item in order.Items.Where(item => item.Product is not null))
            {
                item.Product!.Stock += item.Quantity;
            }

            order.Status = OrderStatus.PaymentFailed;
            order.Payment.Status = PaymentStatus.Failed;
            order.Payment.FailureReason = reason;
            order.Payment.CompletedAt = DateTime.UtcNow;
        }

        private static void ExpireOrderAndRestoreStock(Order order)
        {
            if (order.Status != OrderStatus.PendingPayment || order.Payment is null)
            {
                return;
            }

            foreach (var item in order.Items.Where(item => item.Product is not null))
            {
                item.Product!.Stock += item.Quantity;
            }

            order.Status = OrderStatus.PaymentExpired;
            order.Payment.Status = PaymentStatus.Expired;
            order.Payment.FailureReason = "Payment session expired.";
            order.Payment.CompletedAt = DateTime.UtcNow;
        }

        private static void ClearPurchasedItemsFromCart(Cart cart, Order order)
        {
            var itemsToRemove = new List<CartItem>();

            foreach (var orderItem in order.Items)
            {
                var cartItem = cart.Items.FirstOrDefault(item => item.ProductId == orderItem.ProductId);
                if (cartItem is null)
                {
                    continue;
                }

                if (cartItem.Quantity <= orderItem.Quantity)
                {
                    itemsToRemove.Add(cartItem);
                    continue;
                }

                cartItem.Quantity -= orderItem.Quantity;
            }

            foreach (var cartItem in itemsToRemove)
            {
                cart.Items.Remove(cartItem);
            }
        }

        private static PaymentResultDto CreatePaymentResult(Order order, bool isSuccess, bool isPending, string message) => new()
        {
            OrderId = order.Id,
            IsSuccess = isSuccess,
            IsPending = isPending,
            Message = message,
            OrderStatus = order.Status.ToString(),
            PaymentStatus = order.Payment?.Status.ToString() ?? "Unavailable",
            TotalAmount = order.TotalAmount
        };

        private static bool IsTrue(string? value) =>
            string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);

        private static string? ExtractLastFour(string? sourcePan)
        {
            if (string.IsNullOrWhiteSpace(sourcePan))
            {
                return null;
            }

            var digits = new string(sourcePan.Where(char.IsDigit).ToArray());
            return digits.Length >= 4 ? digits[^4..] : digits;
        }
    }
}
