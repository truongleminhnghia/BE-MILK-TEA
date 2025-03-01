using System.Collections.Generic;
using System.Threading.Tasks;
using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using Data_Access_Layer;
using Microsoft.EntityFrameworkCore;
using Business_Logic_Layer.Repositories;
using Data_Access_Layer.Repositories;


namespace Business_Logic_Layer.Repositories
{
    public class CartItemRepository : ICartItemRepository
    {
        private readonly ApplicationDbContext _context;

        public CartItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CartItem>> GetAllAsync()
        {
            return await _context.CartItems.ToListAsync();
        }

        public async Task<CartItem> GetByIdAsync(Guid id)
        {
            return await _context.CartItems.FindAsync(id);
        }

        public async Task AddAsync(CartItem cartItem)
        {
            await _context.CartItems.AddAsync(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CartItem cartItem)
        {
            _context.CartItems.Update(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
        }

        public Task<IEnumerable<CartItem>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<CartItem> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task Add(CartItem cartItem)
        {
            throw new NotImplementedException();
        }

        public Task<CartItem> Update(CartItem cartItem)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<CartItem> Create(CartItem cartItem)
        {
            throw new NotImplementedException();
        }
    }
}
