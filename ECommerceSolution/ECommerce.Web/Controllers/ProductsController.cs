using ECommerce.API.Helpers;
using ECommerce.Web.Filters;
using ECommerce.Web.Models;
using ECommerce.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace ECommerce.Web.Controllers
{
    [RequireSession]
    public class ProductsController : Controller
    {
        private readonly IHttpClientFactory _factory;
        private readonly ProductImageStore _productImageStore;

        public ProductsController(IHttpClientFactory factory, ProductImageStore productImageStore)
        {
            _factory = factory;
            _productImageStore = productImageStore;
        }

        public async Task<IActionResult> Index()
        {
            var client = _factory.CreateClient("ApiClient");
            var products = await client.GetFromJsonAsync<List<ProductViewModel>>("products") ?? new List<ProductViewModel>();

            foreach (var product in products)
            {
                product.ImageUrl ??= _productImageStore.GetImageUrl(product.Id);
            }

            ViewBag.IsAdmin = SessionHelper.IsAdmin(HttpContext.Session);
            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var client = _factory.CreateClient("ApiClient");
            var response = await client.GetAsync($"products/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "We couldn't find that product.";
                return RedirectToAction(nameof(Index));
            }

            var product = await response.Content.ReadFromJsonAsync<ProductViewModel>();
            if (product is null)
            {
                TempData["Error"] = "We couldn't load that product.";
                return RedirectToAction(nameof(Index));
            }

            product.ImageUrl ??= _productImageStore.GetImageUrl(product.Id);
            ViewBag.IsAdmin = SessionHelper.IsAdmin(HttpContext.Session);
            return View(product);
        }

        [HttpGet]
        [RequireSession(AdminOnly = true)]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireSession(AdminOnly = true)]
        public async Task<IActionResult> Create(CreateProductViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var token = SessionHelper.GetToken(HttpContext.Session);
            var client = _factory.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PostAsJsonAsync("products", new
            {
                model.Name,
                model.Price,
                model.Stock
            });

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Failed to create product.");
                return View(model);
            }

            var createdProduct = await response.Content.ReadFromJsonAsync<ProductViewModel>();
            if (createdProduct is not null)
            {
                await _productImageStore.SaveImageAsync(createdProduct.Id, model.ImageFile, model.ImageUrl);
            }

            TempData["Success"] = "Product created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [RequireSession(AdminOnly = true)]
        public async Task<IActionResult> Edit(int id)
        {
            var client = _factory.CreateClient("ApiClient");
            var response = await client.GetAsync($"products/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "We couldn't find that product.";
                return RedirectToAction(nameof(Index));
            }

            var product = await response.Content.ReadFromJsonAsync<ProductViewModel>();
            if (product is null)
            {
                TempData["Error"] = "We couldn't load that product.";
                return RedirectToAction(nameof(Index));
            }

            var imageUrl = product.ImageUrl ?? _productImageStore.GetImageUrl(product.Id);

            return View(new EditProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Stock = product.Stock,
                CurrentImageUrl = imageUrl,
                ImageUrl = imageUrl
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireSession(AdminOnly = true)]
        public async Task<IActionResult> Edit(EditProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.CurrentImageUrl ??= _productImageStore.GetImageUrl(model.Id);
                return View(model);
            }

            var token = SessionHelper.GetToken(HttpContext.Session);
            var client = _factory.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PutAsJsonAsync($"products/{model.Id}", new
            {
                model.Name,
                model.Price,
                model.Stock
            });

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Failed to update product.");
                model.CurrentImageUrl ??= _productImageStore.GetImageUrl(model.Id);
                return View(model);
            }

            await _productImageStore.SaveImageAsync(model.Id, model.ImageFile, model.ImageUrl);

            TempData["Success"] = "Product updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireSession(AdminOnly = true)]
        public async Task<IActionResult> Delete(int id)
        {
            var token = SessionHelper.GetToken(HttpContext.Session);
            var client = _factory.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            await client.DeleteAsync($"products/{id}");
            TempData["Success"] = "Product deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
