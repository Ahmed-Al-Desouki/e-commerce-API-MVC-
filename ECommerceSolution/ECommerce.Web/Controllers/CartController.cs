using ECommerce.API.Helpers;
using ECommerce.Web.Filters;
using ECommerce.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace ECommerce.Web.Controllers
{
    [RequireSession]
    public class CartController : Controller
    {
        private readonly IHttpClientFactory _factory;

        public CartController(IHttpClientFactory factory)
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
            var client = CreateAuthorizedClient();
            var cart = await client.GetFromJsonAsync<CartViewModel>("cart");
            return View(cart ?? new CartViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddToCartViewModel model, string? returnUrl)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please choose a valid quantity.";
                return RedirectToAction("Index", "Products");
            }

            var client = CreateAuthorizedClient();
            await client.PostAsJsonAsync("cart/items", model);

            TempData["Success"] = "Product added to cart.";

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Clear()
        {
            var client = CreateAuthorizedClient();
            await client.DeleteAsync("cart");
            TempData["Success"] = "Cart cleared.";
            return RedirectToAction(nameof(Index));
        }
    }
}
