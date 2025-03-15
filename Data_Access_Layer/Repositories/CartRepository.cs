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
    public interface ICartRepository
    {
        Task<Cart> GetOrCreateCartAsync(Guid accountId);
        Task<Cart> CreateAsync(Cart cart);
        //Task<Cart?> UpdateAsync(Guid id, Cart cart);
    }
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _context;
        public CartRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Cart> CreateAsync(Cart cart)
        {
            cart.Id = Guid.NewGuid();
            cart.CreateAt = DateTime.UtcNow;

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

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

        //public async Task<Cart?> UpdateAsync(Guid id, Cart cart)
        //{
        //    var existingCart = await _context.Carts.FindAsync(id);
        //    if (existingCart == null)
        //    {
        //        return null;
        //    }
        //    existingCart.UpdateAt = DateTime.Now;

        //    await _context.SaveChangesAsync();
        //    return existingCart;
        //}

    }
    }
