using ECommerce.Application.DTOs.Cart;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;

        public CartService(ICartRepository cartRepository, IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public async Task<CartDto> GetCartAsync(int userId)
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId);
            if (cart is null)
                return new CartDto();

            return new CartDto
            {
                CartId = cart.Id,
                Items = cart.Items.Select(i => new CartItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    UnitPrice = i.Product.Price,
                    Quantity = i.Quantity
                }).ToList()
            };
        }

        public async Task AddToCartAsync(int userId, AddToCartDto dto)
        {
            var product = await _productRepository.GetByIdAsync(dto.ProductId);
            if (product is null)
                throw new KeyNotFoundException("Product not found.");

            if (product.Stock < dto.Quantity)
                throw new InvalidOperationException("Insufficient stock.");

            var cart = await _cartRepository.GetByUserIdAsync(userId)
                       ?? new Cart { UserId = userId };

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);

            if (existingItem is not null)
                existingItem.Quantity += dto.Quantity;
            else
                cart.Items.Add(new CartItem { ProductId = dto.ProductId, Quantity = dto.Quantity });

            if (cart.Id == 0)
                await _cartRepository.AddAsync(cart);

            await _cartRepository.SaveChangesAsync();
        }

        public async Task ClearCartAsync(int userId)
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId);
            if (cart is null) return;

            cart.Items.Clear();
            await _cartRepository.SaveChangesAsync();
        }
    }
}
