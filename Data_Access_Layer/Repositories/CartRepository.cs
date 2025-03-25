using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data_Access_Layer.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _context;
        public CartRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Cart> FindByAccount(Guid id)
        {
            return await _context.Carts
               .Include(c => c.Account)
               .Include(c => c.CartItems.Where(ci => ci.IsCart == false).OrderByDescending(ci => ci.CreateAt)) // Lọc và sắp xếp
                   .ThenInclude(ci => ci.Ingredient)
               .Include(c => c.CartItems)
                   .ThenInclude(ci => ci.Ingredient.Images)
               .Include(c => c.CartItems)
                   .ThenInclude(ci => ci.Ingredient.IngredientQuantities)
               .FirstOrDefaultAsync(c => c.AccountId == id);
        }

        public async Task<Cart> FindById(Guid id)
        {
            return await _context.Carts
       .Include(c => c.Account)
       .Include(c => c.CartItems)
           .ThenInclude(ci => ci.Ingredient)
       .Include(c => c.CartItems)
           .ThenInclude(ci => ci.Ingredient.Images)
       .Include(c => c.CartItems)
           .ThenInclude(ci => ci.Ingredient.IngredientQuantities).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Cart> Save(Cart cart)
        {
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
            return cart;
        }
    }
}