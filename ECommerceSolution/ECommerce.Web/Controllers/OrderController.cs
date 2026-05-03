using ECommerce.API.Helpers;
using ECommerce.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ECommerce.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IHttpClientFactory _factory;

        public OrderController(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        private HttpClient CreateAuthorizedClient()
        {
            var token = SessionHelper.GetToken(HttpContext.Session);
            var client = _factory.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        public async Task<IActionResult> Index()
        {
            if (!SessionHelper.IsLoggedIn(HttpContext.Session))
            {
                return RedirectToAction("Login", "Auth");
            }

            var client = CreateAuthorizedClient();
            var orders = await client.GetFromJsonAsync<List<OrderViewModel>>("order");
            return View(orders ?? new List<OrderViewModel>());
        }

        [HttpPost]
        public async Task<IActionResult> Checkout()
        {
            if (!SessionHelper.IsLoggedIn(HttpContext.Session))
            {
                return RedirectToAction("Login", "Auth");
            }

            var client = CreateAuthorizedClient();
            var cart = await client.GetFromJsonAsync<CartViewModel>("cart") ?? new CartViewModel();

            if (!cart.Items.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }

            return View(new CheckoutViewModel
            {
                Cart = cart
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartPayment(CheckoutViewModel model, CancellationToken cancellationToken)
        {
            if (!SessionHelper.IsLoggedIn(HttpContext.Session))
            {
                return RedirectToAction("Login", "Auth");
            }

            var client = CreateAuthorizedClient();
            var cart = await client.GetFromJsonAsync<CartViewModel>("cart", cancellationToken) ?? new CartViewModel();
            model.Cart = cart;

            if (!ModelState.IsValid)
            {
                return View("Checkout", model);
            }

            if (!cart.Items.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }

            var request = new CheckoutRequestApiModel
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                City = model.City,
                State = model.State,
                Street = model.Street,
                Building = model.Building,
                Apartment = string.IsNullOrWhiteSpace(model.Apartment) ? "NA" : model.Apartment,
                Floor = string.IsNullOrWhiteSpace(model.Floor) ? "NA" : model.Floor,
                Country = string.IsNullOrWhiteSpace(model.Country) ? "EG" : model.Country,
                ReturnUrl = Url.Action(nameof(PaymentCallback), "Order", null, Request.Scheme) ?? string.Empty
            };

            var response = await client.PostAsJsonAsync("order/checkout/initiate", request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = await ExtractErrorMessageAsync(response, "Unable to start the payment flow.");
                return View("Checkout", model);
            }

            var session = await response.Content.ReadFromJsonAsync<CheckoutSessionViewModel>(cancellationToken: cancellationToken);
            if (session is null || string.IsNullOrWhiteSpace(session.CheckoutUrl))
            {
                TempData["Error"] = "Paymob checkout URL was not returned.";
                return View("Checkout", model);
            }

            return Redirect(session.CheckoutUrl);
        }

        [HttpGet]
        public async Task<IActionResult> PaymentCallback(CancellationToken cancellationToken)
        {
            var client = _factory.CreateClient("ApiClient");
            var callback = new PaymentCallbackApiModel
            {
                Hmac = Request.Query["hmac"],
                MerchantOrderId = Request.Query["merchant_order_id"],
                Order = Request.Query["order"],
                Id = Request.Query["id"],
                AmountCents = Request.Query["amount_cents"],
                CreatedAt = Request.Query["created_at"],
                Currency = Request.Query["currency"],
                ErrorOccured = Request.Query["error_occured"],
                HasParentTransaction = Request.Query["has_parent_transaction"],
                IntegrationId = Request.Query["integration_id"],
                Is3DSecure = Request.Query["is_3d_secure"],
                IsAuth = Request.Query["is_auth"],
                IsCapture = Request.Query["is_capture"],
                IsRefunded = Request.Query["is_refunded"],
                IsStandalonePayment = Request.Query["is_standalone_payment"],
                IsVoided = Request.Query["is_voided"],
                Owner = Request.Query["owner"],
                Pending = Request.Query["pending"],
                Success = Request.Query["success"],
                SourceDataPan = Request.Query["source_data.pan"],
                SourceDataSubType = Request.Query["source_data.sub_type"],
                SourceDataType = Request.Query["source_data.type"]
            };

            var response = await client.PostAsJsonAsync("order/payment/response-callback", callback, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return View("PaymentResult", new PaymentResultViewModel
                {
                    Message = await ExtractErrorMessageAsync(response, "Payment could not be verified."),
                    OrderStatus = "Unknown",
                    PaymentStatus = "Unknown"
                });
            }

            var result = await response.Content.ReadFromJsonAsync<PaymentResultViewModel>(cancellationToken: cancellationToken);
            return View("PaymentResult", result ?? new PaymentResultViewModel
            {
                Message = "Payment finished but no result payload was returned."
            });
        }

        private static async Task<string> ExtractErrorMessageAsync(HttpResponseMessage response, string fallbackMessage)
        {
            try
            {
                var payload = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                return string.IsNullOrWhiteSpace(payload?.Message) ? fallbackMessage : payload.Message;
            }
            catch (JsonException)
            {
                return fallbackMessage;
            }
        }

        private sealed class ApiErrorResponse
        {
            public string? Message { get; set; }
        }
    }
}
