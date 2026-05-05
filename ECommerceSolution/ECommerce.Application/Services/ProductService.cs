using ECommerce.Application.DTOs.Product;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return products.Select(MapToDto);
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            ValidateProductId(id);

            var product = await _productRepository.GetByIdAsync(id);
            return product is null ? null : MapToDto(product);
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            ValidateCreateDto(dto);

            var product = new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                Stock = dto.Stock
            };

            // No check Null, empty, and negative for price and stock ya dessokei

            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();

            return MapToDto(product);
        }

        public async Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto)
        {
            ValidateProductId(id);
            ValidateUpdateDto(dto);

            var product = await _productRepository.GetByIdAsync(id);
            if (product is null)
                throw new KeyNotFoundException($"Product with id {id} not found.");

            product.Name = dto.Name;
            product.Price = dto.Price;
            product.Stock = dto.Stock;

            await _productRepository.SaveChangesAsync();

            return MapToDto(product);
        }

        public async Task DeleteAsync(int id)
        {
            ValidateProductId(id);

            var product = await _productRepository.GetByIdAsync(id);
            if (product is null)
                throw new KeyNotFoundException($"Product with id {id} not found.");

            await _productRepository.DeleteAsync(product);
            await _productRepository.SaveChangesAsync();
        }

        private static ProductDto MapToDto(Product product) => new()
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Stock = product.Stock
        };

        private static void ValidateProductId(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "Product id must be greater than zero.");
        }

        private static void ValidateCreateDto(CreateProductDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);
            ValidateProductValues(dto.Name, dto.Price, dto.Stock);
        }

        private static void ValidateUpdateDto(UpdateProductDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);
            ValidateProductValues(dto.Name, dto.Price, dto.Stock);
        }

        private static void ValidateProductValues(string? name, decimal price, int stock)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name is required.");

            if (price < 0)
                throw new ArgumentOutOfRangeException(nameof(price), "Product price cannot be negative.");

            if (stock < 0)
                throw new ArgumentOutOfRangeException(nameof(stock), "Product stock cannot be negative.");
        }
    }
}
