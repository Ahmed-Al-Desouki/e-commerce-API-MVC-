using ECommerce.API.Helpers;
using ECommerce.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory _factory;

        public AuthController(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var client = _factory.CreateClient("ApiClient");
            var response = await client.PostAsJsonAsync("auth/login", model);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Invalid email or password.";
                return View(model);
            }

            var result = await response.Content.ReadFromJsonAsync<AuthResponseModel>();
            if (result is null) return View(model);

            SessionHelper.SetUserSession(HttpContext.Session,
                result.Token, result.Username, result.IsAdmin, result.UserId);

            return RedirectToAction("Index", "Products");
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            var client = _factory.CreateClient("ApiClient");
            var response = await client.PostAsJsonAsync("auth/register", model);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Registration failed. Email may already be in use.";
                return View(model);
            }

            var result = await response.Content.ReadFromJsonAsync<AuthResponseModel>();
            if (result is null) return View(model);

            SessionHelper.SetUserSession(HttpContext.Session,
                result.Token, result.Username, result.IsAdmin, result.UserId);

            return RedirectToAction("Index", "Products");
        }

        public IActionResult Logout()
        {
            SessionHelper.Clear(HttpContext.Session);
            return RedirectToAction("Login");
        }
    }
}
