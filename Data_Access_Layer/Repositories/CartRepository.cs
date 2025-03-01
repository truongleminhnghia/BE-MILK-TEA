using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Data_Access_Layer.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _context;

        public CartRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Cart> Create(Cart cart)  // Đổi từ AddAsync() -> Create()
        {
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task<Cart?> GetByAccountIdAsync(Guid accountId)
        {
            return await _context.Carts.FirstOrDefaultAsync(c => c.AccountId == accountId);
        }
    }
}
