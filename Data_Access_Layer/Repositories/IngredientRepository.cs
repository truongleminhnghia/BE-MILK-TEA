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
    public class IngredientRepository : IIngredientRepository
    {
        private readonly ApplicationDbContext _context;

        public IngredientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CategoryExistsAsync(Guid categoryId)
        {
            return await _context.Categories.AnyAsync(c => c.Id == categoryId);
        }

        public async Task<IEnumerable<Ingredient>> GetAllAsync()
        {
            return await _context
                .Ingredients.Include(i => i.Category)
                .Include(i => i.Images)
                .ToListAsync();
        }

        public async Task<Ingredient> GetByIdAsync(Guid id)
        {
            return await _context
                .Ingredients.Include(i => i.Category)
                .Include(i => i.Images)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Ingredient> CreateAsync(Ingredient ingredient)
        {
            _context.Ingredients.Add(ingredient);
            await _context.SaveChangesAsync();
            return ingredient;
        }

        public async Task<Ingredient> UpdateAsync(Ingredient ingredient)
        {
            var existingIngredient = await _context.Ingredients.FindAsync(ingredient.Id);
            if (existingIngredient != null)
            {
                _context.Entry(existingIngredient).State = EntityState.Detached;
            }

            _context.Entry(ingredient).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return ingredient;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient == null)
                return false;

            _context.Ingredients.Remove(ingredient);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Ingredient> GetLastIngredientCode()
        {
            return await _context
                .Ingredients.OrderByDescending(i => i.IngredientCode)
                .FirstOrDefaultAsync();
        }
    }
}
