﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using Microsoft.EntityFrameworkCore;


namespace Data_Access_Layer.Repositories
{
    public interface ICartItemRepository
    {
        Task<List<CartItem>> GetCartItemsAsync();
        Task<CartItem?> GetByIdAsync(Guid id);
        Task<CartItem> CreateAsync(CartItem cartItem);
        Task<CartItem?> UpdateAsync(Guid id, CartItem cartItem);
        Task<bool> DeleteByIdAsync(Guid id);
    }
    public class CartItemRepository : ICartItemRepository
    {
        private readonly ApplicationDbContext _context;
        public CartItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<CartItem> CreateAsync(CartItem cartItem)
        {
            cartItem.Id = Guid.NewGuid();
            cartItem.CreateAt = DateTime.Now;

            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();
            return cartItem;
        }

        public async Task<bool> DeleteByIdAsync(Guid id)
        {
            var existingCartItem = await _context.CartItems.FindAsync(id);
            if (existingCartItem == null)
            {
                return false;
            }

            _context.CartItems.Remove(existingCartItem);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<CartItem>> GetCartItemsAsync()
        {
            return await _context.CartItems.ToListAsync();
        }

        public async Task<CartItem?> GetByIdAsync(Guid id)
        {
            return await _context.CartItems.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<CartItem?> UpdateAsync(Guid id, CartItem cartItem)
        {
            var existingCartItem = await _context.CartItems.FindAsync(id);
            if (existingCartItem == null)
            {
                return null;
            }

            existingCartItem.Quantity = cartItem.Quantity;        
            existingCartItem.UpdateAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return existingCartItem;
        }

        
    }
}
