using System.Text.Json;

namespace ECommerce.Web.Services
{
    public class ProductImageStore
    {
        private readonly string _metadataPath;
        private readonly string _uploadFolderPath;
        private readonly SemaphoreSlim _syncLock = new(1, 1);
        private Dictionary<int, string> _images = new();

        public ProductImageStore(IWebHostEnvironment environment)
        {
            _metadataPath = Path.Combine(environment.ContentRootPath, "App_Data", "product-images.json");
            _uploadFolderPath = Path.Combine(environment.WebRootPath, "uploads", "products");

            Directory.CreateDirectory(Path.GetDirectoryName(_metadataPath)!);
            Directory.CreateDirectory(_uploadFolderPath);

            if (File.Exists(_metadataPath))
            {
                var json = File.ReadAllText(_metadataPath);
                _images = JsonSerializer.Deserialize<Dictionary<int, string>>(json) ?? new Dictionary<int, string>();
            }
        }

        public string? GetImageUrl(int productId)
        {
            return _images.TryGetValue(productId, out var imageUrl) ? imageUrl : null;
        }

        public async Task SaveImageAsync(int productId, IFormFile? imageFile, string? imageUrl)
        {
            var normalizedUrl = imageUrl?.Trim();

            if (imageFile is not null && imageFile.Length > 0)
            {
                var extension = Path.GetExtension(imageFile.FileName);
                if (string.IsNullOrWhiteSpace(extension))
                {
                    extension = ".jpg";
                }

                var fileName = $"product-{productId}-{Guid.NewGuid():N}{extension}";
                var physicalPath = Path.Combine(_uploadFolderPath, fileName);

                await using var stream = File.Create(physicalPath);
                await imageFile.CopyToAsync(stream);

                normalizedUrl = $"/uploads/products/{fileName}";
            }

            if (string.IsNullOrWhiteSpace(normalizedUrl))
            {
                return;
            }

            await _syncLock.WaitAsync();
            try
            {
                _images[productId] = normalizedUrl;
                var json = JsonSerializer.Serialize(_images, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_metadataPath, json);
            }
            finally
            {
                _syncLock.Release();
            }
        }
    }
}
