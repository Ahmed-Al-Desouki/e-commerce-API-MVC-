using System.Globalization;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using ECommerce.Application.DTOs.Order;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Infrastructure.Configurations;
using Microsoft.Extensions.Options;

namespace ECommerce.Infrastructure.Services
{
    public class PaymobGateway : IPaymentGateway
    {
        private readonly HttpClient _httpClient;
        private readonly PaymobOptions _options;

        public PaymobGateway(HttpClient httpClient, IOptions<PaymobOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<PaymentGatewaySessionDto> CreateCardPaymentAsync(PaymentGatewayRequestDto request, CancellationToken cancellationToken = default)
        {
            var authToken = await GetAuthTokenAsync(cancellationToken);
            var amountCents = ToAmountCents(request.Amount);

            var orderResponse = await PostAsync<PaymobOrderResponse>(
                "ecommerce/orders",
                new
                {
                    auth_token = authToken,
                    delivery_needed = false,
                    amount_cents = amountCents,
                    currency = request.Currency,
                    merchant_order_id = request.MerchantOrderId,
                    items = request.Items.Select(item => new
                    {
                        name = item.Name,
                        amount_cents = ToAmountCents(item.UnitPrice),
                        description = item.Name,
                        quantity = item.Quantity
                    })
                },
                cancellationToken);

            var paymentKeyResponse = await PostAsync<PaymobPaymentKeyResponse>(
                "acceptance/payment_keys",
                new
                {
                    auth_token = authToken,
                    amount_cents = amountCents,
                    expiration = _options.PaymentKeyExpirationSeconds,
                    order_id = orderResponse.Id,
                    billing_data = new
                    {
                        apartment = request.Apartment,
                        email = request.Email,
                        floor = request.Floor,
                        first_name = request.FirstName,
                        street = request.Street,
                        building = request.Building,
                        phone_number = request.PhoneNumber,
                        shipping_method = "PKG",
                        postal_code = "00000",
                        city = request.City,
                        country = request.Country,
                        last_name = request.LastName,
                        state = request.State
                    },
                    currency = request.Currency,
                    integration_id = _options.IntegrationId.Card,
                    lock_order_when_paid = false
                },
                cancellationToken);

            return new PaymentGatewaySessionDto
            {
                ProviderOrderId = orderResponse.Id.ToString(CultureInfo.InvariantCulture),
                PaymentToken = paymentKeyResponse.Token,
                CheckoutUrl = $"{_options.BaseUrl.TrimEnd('/')}/acceptance/iframes/{_options.IframeId.Card}?payment_token={paymentKeyResponse.Token}"
            };
        }

        public bool IsCallbackAuthentic(PaymentCallbackDto callback)
        {
            if (string.IsNullOrWhiteSpace(callback.Hmac))
            {
                return false;
            }

            var payload = string.Concat(
                callback.AmountCents ?? string.Empty,
                callback.CreatedAt ?? string.Empty,
                callback.Currency ?? string.Empty,
                callback.ErrorOccured ?? string.Empty,
                callback.HasParentTransaction ?? string.Empty,
                callback.Id ?? string.Empty,
                callback.IntegrationId ?? string.Empty,
                callback.Is3DSecure ?? string.Empty,
                callback.IsAuth ?? string.Empty,
                callback.IsCapture ?? string.Empty,
                callback.IsRefunded ?? string.Empty,
                callback.IsStandalonePayment ?? string.Empty,
                callback.IsVoided ?? string.Empty,
                callback.Order ?? string.Empty,
                callback.Owner ?? string.Empty,
                callback.Pending ?? string.Empty,
                callback.SourceDataPan ?? string.Empty,
                callback.SourceDataSubType ?? string.Empty,
                callback.SourceDataType ?? string.Empty,
                callback.Success ?? string.Empty);

            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(_options.HmacSecret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            var computed = Convert.ToHexString(hash).ToLowerInvariant();

            return string.Equals(computed, callback.Hmac, StringComparison.OrdinalIgnoreCase);
        }

        private async Task<string> GetAuthTokenAsync(CancellationToken cancellationToken)
        {
            var response = await PostAsync<PaymobAuthResponse>(
                "auth/tokens",
                new { api_key = _options.ApiKey },
                cancellationToken);

            return response.Token;
        }

        private async Task<TResponse> PostAsync<TResponse>(string path, object payload, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsJsonAsync(path, payload, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var details = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new InvalidOperationException($"Paymob request to '{path}' failed with status code {(int)response.StatusCode}. Response: {details}");
            }

            var body = await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken);
            if (body is null)
            {
                throw new InvalidOperationException($"Paymob request to '{path}' returned an empty response.");
            }

            return body;
        }

        private static int ToAmountCents(decimal amount) =>
            (int)Math.Round(amount * 100m, MidpointRounding.AwayFromZero);

        private sealed class PaymobAuthResponse
        {
            public string Token { get; set; } = string.Empty;
        }

        private sealed class PaymobOrderResponse
        {
            public long Id { get; set; }
        }

        private sealed class PaymobPaymentKeyResponse
        {
            public string Token { get; set; } = string.Empty;
        }
    }
}
