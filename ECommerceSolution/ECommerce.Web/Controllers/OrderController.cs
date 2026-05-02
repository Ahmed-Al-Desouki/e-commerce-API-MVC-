using ECommerce.API.Helpers;
using ECommerce.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

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
                return RedirectToAction("Login", "Auth");

            var client = CreateAuthorizedClient();
            var orders = await client.GetFromJsonAsync<List<OrderViewModel>>("order");
            return View(orders ?? new List<OrderViewModel>());
        }

        [HttpPost]
        public async Task<IActionResult> Checkout()
        {
            var client = CreateAuthorizedClient();
            var response = await client.PostAsync("order/checkout", null);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Checkout failed. Your cart may be empty.";
                return RedirectToAction("Index", "Cart");
            }

            return RedirectToAction("Index");
        }
    }
}
