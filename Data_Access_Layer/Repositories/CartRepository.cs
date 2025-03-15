using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using Microsoft.EntityFrameworkCore;


namespace Data_Access_Layer.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _context;

        public async Task<Cart> CreateAsync(Cart cart)
        {
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task<Cart?> GetByAccountAsync(Guid id)
        {
            return await _context.Carts
            .Include(c => c.Account)
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.AccountId == id) ?? null;
        }

        public async Task<Cart?> GetByIdAsync(Guid id)
        {
            return await _context.Carts
            .Include(c => c.Account)
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.Id == id) ?? null;
        }

        public async Task<Cart?> UpdateAsync(Guid id, Cart cart)
        {
            var existingCart = await _context.Carts.FindAsync(id);
            if (existingCart == null)
            {
                return null;
            }
            existingCart.UpdateAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return existingCart;
        }
    }
}
