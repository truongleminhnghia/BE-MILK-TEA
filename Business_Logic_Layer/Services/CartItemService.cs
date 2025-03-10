﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;

namespace Business_Logic_Layer.Services
{
    public interface ICartItemService
    {
        Task<List<CartItem>> GetCartItemsAsync();
        Task<CartItem?> GetByIdAsync(Guid id);
        Task<CartItem> CreateAsync(CartItem cartItem);
        Task<CartItem?> UpdateAsync(Guid id, CartItem cartItem);
        Task<bool> DeleteByIdAsync(Guid id);
    }
    public class CartItemService : ICartItemService
    {
        private readonly ICartItemRepository _cartItemRepository;
        public CartItemService(ICartItemRepository cartItemRepository)
        {
            _cartItemRepository = cartItemRepository;
        }
        public async Task<CartItem> CreateAsync(CartItem cartItem)
        {
            return await _cartItemRepository.CreateAsync(cartItem);
        }

        public async Task<bool> DeleteByIdAsync(Guid id)
        {
            return await _cartItemRepository.DeleteByIdAsync(id);
        }

        public async Task<CartItem?> GetByIdAsync(Guid id)
        {
            return await _cartItemRepository.GetByIdAsync(id);
        }

        public async Task<List<CartItem>> GetCartItemsAsync()
        {
            return await _cartItemRepository.GetCartItemsAsync();
        }

        public async Task<CartItem?> UpdateAsync(Guid id, CartItem cartItem)
        {
            return await _cartItemRepository.UpdateAsync(id, cartItem);
        }
    }
}
