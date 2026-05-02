using ECommerce.API.Helpers;
using ECommerce.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace ECommerce.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IHttpClientFactory _factory;

        public ProductsController(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _factory.CreateClient("ApiClient");
            var products = await client.GetFromJsonAsync<List<ProductViewModel>>("products");
            ViewBag.IsAdmin = SessionHelper.IsAdmin(HttpContext.Session);
            ViewBag.IsLoggedIn = SessionHelper.IsLoggedIn(HttpContext.Session);
            return View(products ?? new List<ProductViewModel>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!SessionHelper.IsAdmin(HttpContext.Session))
                return RedirectToAction("Login", "Auth");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductViewModel model)
        {
            var token = SessionHelper.GetToken(HttpContext.Session);
            var client = _factory.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PostAsJsonAsync("products", model);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Failed to create product.";
                return View(model);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var token = SessionHelper.GetToken(HttpContext.Session);
            var client = _factory.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            await client.DeleteAsync($"products/{id}");
            return RedirectToAction("Index");
        }
    }
}
