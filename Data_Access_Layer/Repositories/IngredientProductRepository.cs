using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Repositories
{
    public class IngredientProductRepository : IIngredientProductRepository
    {
        private readonly ApplicationDbContext _context;

        public IngredientProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IngredientProduct> CreateAsync(IngredientProduct ingredientProduct)
        {
            _context.IngredientProducts.Add(ingredientProduct);
            await _context.SaveChangesAsync();
            return ingredientProduct;
        }

        public async Task UpdateAsync(IngredientProduct ingredientProduct)
        {
            _context.IngredientProducts.Update(ingredientProduct);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IngredientExistsAsync(Guid ingredientId)
        {
            return await _context.Ingredients.AnyAsync(i => i.Id == ingredientId);
        }

        public async Task<IngredientProduct> GetIngredientProductbyId(Guid ingredientProductId)
        {

            return await _context.IngredientProducts.FirstOrDefaultAsync(n => n.Id.Equals(ingredientProductId));
        }
    }
}
