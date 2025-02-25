using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using Microsoft.EntityFrameworkCore;
using Mysqlx.Crud;
using Org.BouncyCastle.Asn1.X509;

namespace Data_Access_Layer.Repositories
{
    public interface ICartRepository
    {
        Task<Cart?> GetByIdAsync(Guid id);
        Task<Cart> CreateAsync(Cart cart);
        Task<Cart?> UpdateAsync(Guid id, Cart cart);
    }
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _context;

        public async Task<Cart> CreateAsync(Cart cart)
        {
            cart.Id = Guid.NewGuid();
            cart.CreateAt = DateTime.Now;

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task<Cart?> GetByIdAsync(Guid id)
        {
            return await _context.Carts.FirstOrDefaultAsync(o => o.Id == id) ?? null;
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
