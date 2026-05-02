using ECommerce.API.Helpers;
using ECommerce.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace ECommerce.Web.Controllers
{
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
            if (!SessionHelper.IsLoggedIn(HttpContext.Session))
                return RedirectToAction("Login", "Auth");

            var client = CreateAuthorizedClient();
            var cart = await client.GetFromJsonAsync<CartViewModel>("cart");
            return View(cart ?? new CartViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddToCartViewModel model)
        {
            if (!SessionHelper.IsLoggedIn(HttpContext.Session))
                return RedirectToAction("Login", "Auth");

            var client = CreateAuthorizedClient();
            await client.PostAsJsonAsync("cart/items", model);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Clear()
        {
            var client = CreateAuthorizedClient();
            await client.DeleteAsync("cart");
            return RedirectToAction("Index");
        }
    }
}
