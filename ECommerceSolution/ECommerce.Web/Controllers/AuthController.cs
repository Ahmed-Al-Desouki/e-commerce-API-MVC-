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
        public IActionResult Login(string? returnUrl = null)
        {
            if (SessionHelper.IsLoggedIn(HttpContext.Session))
                return RedirectToAction("Index", "Products");

            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var client = _factory.CreateClient("ApiClient");
                var response = await client.PostAsJsonAsync("auth/login", new
                {
                    model.Email,
                    model.Password
                });

                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(string.Empty, "Invalid email or password.");
                    return View(model);
                }

                var result = await response.Content.ReadFromJsonAsync<AuthResponseModel>();
                if (result is null)
                {
                    ModelState.AddModelError(string.Empty, "Login failed. Please try again.");
                    return View(model);
                }

                SessionHelper.SetUserSession(HttpContext.Session,
                    result.Token, result.Username, result.IsAdmin, result.UserId);

                TempData["Success"] = $"Welcome back, {result.Username}!";

                if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    return Redirect(model.ReturnUrl);

                return RedirectToAction("Index", "Products");
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "The API is not reachable right now. Start the API project, then try logging in again.");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (SessionHelper.IsLoggedIn(HttpContext.Session))
                return RedirectToAction("Index", "Products");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var client = _factory.CreateClient("ApiClient");
                var response = await client.PostAsJsonAsync("auth/register", model);

                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(string.Empty, "Registration failed. Email may already be in use.");
                    return View(model);
                }

                var result = await response.Content.ReadFromJsonAsync<AuthResponseModel>();
                if (result is null)
                {
                    ModelState.AddModelError(string.Empty, "Registration failed. Please try again.");
                    return View(model);
                }

                SessionHelper.SetUserSession(HttpContext.Session,
                    result.Token, result.Username, result.IsAdmin, result.UserId);

                TempData["Success"] = "Your account has been created.";
                return RedirectToAction("Index", "Products");
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "The API is not reachable right now. Start the API project, then try registering again.");
                return View(model);
            }
        }

        public IActionResult Logout()
        {
            SessionHelper.Clear(HttpContext.Session);
            TempData["Success"] = "You have been logged out.";
            return RedirectToAction("Login");
        }
    }
}
