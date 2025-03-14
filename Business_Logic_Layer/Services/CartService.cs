﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;
using Microsoft.EntityFrameworkCore;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;


namespace Business_Logic_Layer.Services
{
    public interface ICartService
    {
        public Task<CartResponse?> GetByIdAsync(Guid accountId);
        public Task AddToCartAsync(Guid accountId, Guid ingredientProductId, int quantity);
        public Task<bool?> RemoveItemAsync(Guid accountId, Guid ingredientProductId);
    }
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
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.AccountId == accountId);

            if (cart == null)
            {
                cart = new Cart { AccountId = accountId };
                cart = await _cartRepository.CreateAsync(cart);
            }

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
                Console.WriteLine("error: " + ex.Message);
                }
            }
        }

        public async Task<CartResponse?> GetByIdAsync(Guid accountId)
        {
            var cart = await _cartRepository.GetByIdAsync(accountId);
            if(cart == null)
            {
                return null;
            }
            return new CartResponse
            {
                CartId = cart.Id,
                AccountId = accountId,
                CartItems = cart.CartItems?.Select(ci => new CartItemResponse
                {
                    CartItemId=ci.Id,
                    IngredientProductId=ci.IngredientProductId,
                    Quantity=ci.Quantity
                }).ToList() ?? new List<CartItemResponse>()
            };
            
        }

        public async Task<bool?> RemoveItemAsync(Guid accountId, Guid ingredientProductId)
        {
            var cart = await _cartRepository.GetByIdAsync(accountId);

            if (cart == null) return false;

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.IngredientProductId == ingredientProductId);

            if (cartItem != null)
            {
                _cartItemRepository.RemoveCartItemByIdAsync(cartItem.Id);
                return true;
            }

            return false;
        }
    }
}
