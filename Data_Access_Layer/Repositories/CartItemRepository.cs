using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data_Access_Layer.Repositories
{
    public class CartItemRepository : ICartItemRepository
    {
        private readonly ApplicationDbContext _context;
        public CartItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteCartItem(CartItem cartItem)
        {
            _context.CartItems.Remove(cartItem);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<CartItem>> GetByCart(Guid cartId)
        {
            return await _context.CartItems
         .Include(ci => ci.Ingredient)
             .ThenInclude(i => i.Images) // Nếu Ingredient có danh sách hình ảnh
         .Include(ci => ci.Ingredient)
             .ThenInclude(i => i.IngredientQuantities) // Nếu cần danh sách số lượng liên quan
         .Include(ci => ci.Cart)
         .Where(ci => ci.CartId == cartId)
         .ToListAsync();
        }

        public async Task<CartItem> GetById(Guid id)
        {
            return await _context.CartItems.Include(ci => ci.Ingredient).Include(ci => ci.Cart).FirstOrDefaultAsync(ci => ci.Id == id);
        }

        public async Task<CartItem> SaveCartItem(CartItem cartItem)
        {
            try
            {
                _context.CartItems.Add(cartItem);
                await _context.SaveChangesAsync();
                return cartItem;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.InnerException?.Message);
                throw;
            }
        }

        public async Task<bool> UpdateCartItem(CartItem cartItem)
        {
            _context.CartItems.Update(cartItem);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}