using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Business_Logic_Layer.DTO;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;
using Microsoft.EntityFrameworkCore;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;


namespace Business_Logic_Layer.Services
{
   
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly ApplicationDbContext _context;

        public CartService(ICartRepository cartRepository, ICartItemRepository cartItemRepository, ApplicationDbContext dbContext)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _context = dbContext;
        }

        public async Task AddToCartAsync(Guid accountId, Guid ingredientProductId, int quantity)
        {
            var cart = await _cartRepository.GetOrCreateCartAsync(accountId);

            var existingCartItem = cart.CartItems.FirstOrDefault(ci => ci.IngredientProductId == ingredientProductId);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += quantity; // Cập nhật số lượng
            }
            else
            {
                try
                {
                    cart.CartItems.Add(new CartItem
                    {
                        CartId = cart.Id,
                        IngredientProductId = ingredientProductId,
                        Quantity = quantity
                    });
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }

        public async Task<CartResponse> GetByIdAsync(Guid accountId)
        {
            var cart = await _cartRepository.GetOrCreateCartAsync(accountId);

            return new CartResponse
            {
                CartId = cart.Id,
                AccountId = accountId,
                CartItems = cart.CartItems?.Select(ci => new CartItemResponse
                {
                    CartItemId = ci.Id,
                    IngredientProductId = ci.IngredientProductId,
                    Quantity = ci.Quantity
                }).ToList() ?? new List<CartItemResponse>()
            };
        }


        public async Task<bool> RemoveItemAsync(Guid accountId, Guid ingredientProductId)
        {
            var cart = await _cartRepository.GetOrCreateCartAsync(accountId);

            if (!cart.CartItems.Any())
                return false;

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.IngredientProductId == ingredientProductId);

            if (cartItem != null)
            {
                await _cartItemRepository.RemoveCartItemByIdAsync(cartItem.Id);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
        public async Task<bool> UpdateCartItemQuantityAsync(Guid accountId, Guid ingredientProductId, int quantity)
        {
            var cart = await _cartRepository.GetOrCreateCartAsync(accountId);
            if (cart.CartItems == null || !cart.CartItems.Any())
                return false;

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.IngredientProductId == ingredientProductId);
            if (cartItem == null)
                return false;

            cartItem.Quantity = quantity;
            await _cartRepository.UpdateCartItemQuantityAsync(accountId, ingredientProductId, quantity);
            return true;
        }

        public async Task<bool> ClearCartAsync(Guid accountId)
        {
            var cart = await _cartRepository.GetOrCreateCartAsync(accountId);
            if (cart.CartItems == null || !cart.CartItems.Any())
                return false;

            await _cartRepository.ClearCartAsync(accountId);
            return true;
        }
    }
}
