using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;
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

        public async Task<IEnumerable<Ingredient>> GetAllAsync(
            string? search, Guid? categoryId, string? sortBy, bool isDescending, int page, int pageSize, DateTime? startDate, DateTime? endDate, IngredientStatus? status)
        {
            var query = _context.Ingredients
                .Include(i => i.Category)
                .Include(i => i.Images)
                .AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(i => i.IngredientName.Contains(search));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(i => i.CategoryId == categoryId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(i => i.IngredientStatus == status.Value);
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                DateTime adjustedEndDate = endDate.Value.Date.AddDays(1).AddTicks(-1); // Includes full day

                query = query.Where(c => c.CreateAt >= startDate.Value && c.CreateAt <= adjustedEndDate);
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                query = isDescending
                    ? query.OrderByDescending(e => EF.Property<object>(e, sortBy))
                    : query.OrderBy(e => EF.Property<object>(e, sortBy));
            }

            return await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
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

        Task IIngredientRepository.GetIngredientByIdAsync(Guid ingredientId)
        {
            throw new NotImplementedException();
        }
    }
}
