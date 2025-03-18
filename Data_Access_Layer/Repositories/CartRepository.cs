using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;


namespace Data_Access_Layer.Repositories
{
    
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _context;
        public CartRepository(ApplicationDbContext context)
        {
            _context = context;
        }

   

        //public async Task<Cart> CreateAsync(Cart cart)
        //{
        //    cart.Id = Guid.NewGuid();
        //    cart.CreateAt = DateTime.UtcNow;

        //    _context.Carts.Add(cart);
        //    await _context.SaveChangesAsync();
        //    return cart;
        //}

        public async Task<Cart> GetOrCreateCartAsync(Guid accountId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.AccountId == accountId);

            if (cart == null)
            {
                // Tạo giỏ hàng mới nếu chưa tồn tại
                cart = new Cart
                {
                    Id = Guid.NewGuid(),
                    AccountId = accountId,
                    CartItems = new List<CartItem>(),
                    CreateAt = DateTime.UtcNow,
                    UpdateAt = DateTime.UtcNow
                };

                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }

        public async Task UpdateCartItemQuantityAsync(Guid accountId, Guid ingredientProductId, int quantity)
        {
            var cart = await GetOrCreateCartAsync(accountId);
            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.IngredientProductId == ingredientProductId);

            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
                cart.UpdateAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }


        public async Task ClearCartAsync(Guid accountId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.AccountId == accountId);

            if (cart != null && cart.CartItems.Any())
            {
                _context.CartItems.RemoveRange(cart.CartItems);
                cart.UpdateAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
        public async Task<Cart?> GetByAccountAsync(Guid id)
        {
            return await _context.Carts
        .Include(c => c.CartItems)               
        .ThenInclude(ci => ci.IngredientProduct)
        .ThenInclude(ip => ip.Ingredient)
        .ThenInclude(i => i.Images) // Include images
        .FirstOrDefaultAsync(c => c.AccountId == id);
        }
        public async Task<Cart> CreateAsync(Cart cart)
        {
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
            return cart;
        }
    }
    }
