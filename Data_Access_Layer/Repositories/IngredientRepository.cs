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

        // public async Task<bool> CategoryExistsAsync(Guid categoryId)
        // {
        //     return await _context.Categories.AnyAsync(c => c.Id == categoryId);
        // }

        public async Task<IEnumerable<Ingredient>> GetAllAsync(
            string? search, Guid? categoryId, string? sortBy, bool isDescending, int page, int pageSize, DateTime? startDate, DateTime? endDate, IngredientStatus? status)
        {
            var query = _context.Ingredients
                .Include(i => i.Category)
                .Include(i => i.Images)
                .AsQueryable();
            // **Filtering by name**
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(i => i.IngredientName.Contains(search));
            }

            // **Filtering by CategoryId**
            if (categoryId.HasValue)
            {
                query = query.Where(i => i.CategoryId == categoryId.Value);
            }

            // **Filtering by IngredientStatus**
            if (status.HasValue)
            {
                query = query.Where(i => i.IngredientStatus == status.Value);
            }

            // **Filtering by date range (CreateAt)**
            if (startDate.HasValue && endDate.HasValue)
            {
                DateTime adjustedEndDate = endDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(i => i.CreateAt >= startDate.Value && i.CreateAt <= adjustedEndDate);
            }
            else if (startDate.HasValue)
            {
                query = query.Where(i => i.CreateAt >= startDate.Value);
            }
            else if (endDate.HasValue)
            {
                DateTime adjustedEndDate = endDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(i => i.CreateAt <= adjustedEndDate);
                isDescending = true; // Force descending order if only endDate is provided
            }

            // **Sorting**
            if (!string.IsNullOrEmpty(sortBy))
            {
                query = isDescending
                    ? query.OrderByDescending(e => EF.Property<object>(e, sortBy))
                    : query.OrderBy(e => EF.Property<object>(e, sortBy));
            }
            else
            {
                query = query.OrderByDescending(i => i.CreateAt); // Default sorting by CreateAt descending
            }

            return await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<Ingredient> GetByIdAsync(Guid id)
        {
            return await _context.Ingredients.FirstAsync(a => a.Id.Equals(id));
        }

        public async Task<Ingredient> CreateAsync(Ingredient ingredient)
        {
            _context.Ingredients.Add(ingredient);
            await _context.SaveChangesAsync();
            return ingredient;
        }

        public async Task<Ingredient> UpdateAsync(Guid id, Ingredient ingredient)
        {
            var existingIngredient = await GetByIdAsync(id);
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

        public async Task<Ingredient> GetIngredientByIdAsync(Guid ingredientId)
        {
            return await _context.Ingredients.FirstAsync(a => a.Id.Equals(ingredientId));
        }

        public async Task<bool> CheckCode(string code)
        {
            return await _context.Ingredients.AnyAsync(a => a.IngredientCode.Equals(code));
        }
    }
}
