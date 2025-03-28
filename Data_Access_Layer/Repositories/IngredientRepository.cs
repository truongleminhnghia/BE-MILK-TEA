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

        // public async Task<IEnumerable<Ingredient>> GetAllAsync(
        //     string? search, Guid? categoryId, string? sortBy, bool isDescending, int page, int pageSize, DateTime? startDate, DateTime? endDate, IngredientStatus? status)
        // {
        //     var query = _context.Ingredients
        //         .Include(i => i.Category)
        //         .Include(i => i.Images)
        //         .AsQueryable();
        //     // **Filtering by name**
        //     if (!string.IsNullOrEmpty(search))
        //     {
        //         query = query.Where(i => i.IngredientName.Contains(search));
        //     }

        //     // **Filtering by CategoryId**
        //     if (categoryId.HasValue)
        //     {
        //         query = query.Where(i => i.CategoryId == categoryId.Value);
        //     }

        //     // **Filtering by IngredientStatus**
        //     if (status.HasValue)
        //     {
        //         query = query.Where(i => i.IngredientStatus == status.Value);
        //     }

        //     // **Filtering by date range (CreateAt)**
        //     if (startDate.HasValue && endDate.HasValue)
        //     {
        //         DateTime adjustedEndDate = endDate.Value.Date.AddDays(1).AddTicks(-1);
        //         query = query.Where(i => i.CreateAt >= startDate.Value && i.CreateAt <= adjustedEndDate);
        //     }
        //     else if (startDate.HasValue)
        //     {
        //         query = query.Where(i => i.CreateAt >= startDate.Value);
        //     }
        //     else if (endDate.HasValue)
        //     {
        //         DateTime adjustedEndDate = endDate.Value.Date.AddDays(1).AddTicks(-1);
        //         query = query.Where(i => i.CreateAt <= adjustedEndDate);
        //         isDescending = true; // Force descending order if only endDate is provided
        //     }

        //     // **Sorting**
        //     if (!string.IsNullOrEmpty(sortBy))
        //     {
        //         query = isDescending
        //             ? query.OrderByDescending(e => EF.Property<object>(e, sortBy))
        //             : query.OrderBy(e => EF.Property<object>(e, sortBy));
        //     }
        //     else
        //     {
        //         query = query.OrderByDescending(i => i.CreateAt); // Default sorting by CreateAt descending
        //     }

        //     return await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        // }

        public IQueryable<Ingredient> GetAll(
            string? search,
            string? categorySearch,
            Guid? categoryId,
            DateOnly? startDate,
            DateOnly? endDate,
            IngredientStatus? status,
            decimal? minPrice,
            decimal? maxPrice,
            bool? isSale,
            IngredientType? ingredientType
        )
        {
            var query = _context
                .Ingredients.Include(i => i.Category)
                .Include(i => i.Images)
                .Include(i => i.IngredientQuantities)
                .Include(i => i.IngredientReviews)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(i => i.IngredientName.Contains(search));
            }

            if (!string.IsNullOrEmpty(categorySearch))
            {
                query = query.Where(i => i.Category.CategoryName.Contains(categorySearch));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(i => i.CategoryId == categoryId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(i => i.IngredientStatus == status.Value);
            }

            if (isSale.HasValue)
            {
                query = query.Where(i => i.IsSale == isSale.Value);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(i => i.PriceOrigin >= (double)minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(i => i.PriceOrigin <= (double)maxPrice.Value);
            }

            if (startDate.HasValue || endDate.HasValue)
            {
                DateTime adjustedStart = startDate?.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue;
                DateTime adjustedEnd = endDate?.ToDateTime(TimeOnly.MaxValue) ?? DateTime.MaxValue;

                query = query.Where(i => i.CreateAt >= adjustedStart && i.CreateAt <= adjustedEnd);
            }
            if (ingredientType.HasValue)
            {
                query = query.Where(i => i.IngredientType == ingredientType.Value);
            }


            return query;
        }

        public async Task<Ingredient> GetByIdOrCode(Guid? id, string? code)
        {
            if (id.HasValue && string.IsNullOrEmpty(code))
            {
                return await _context
                    .Ingredients.Include(i => i.Images)
                    .Include(i => i.Category)
                    .Include(i => i.IngredientQuantities)
                    .Include(i => i.IngredientReviews)
                    .FirstAsync(a => a.Id.Equals(id));
            }
            else if (!id.HasValue && code != null)
            {
                if (code.Substring(0,1) != "P")
                {
                    throw new Exception("Định dạng nguyên liệu bị sai");
                }
                return await _context
                    .Ingredients.Include(i => i.Images)
                    .Include(i => i.Category)
                    .Include(i => i.IngredientQuantities)
                    .Include(i => i.IngredientReviews)
                    .FirstAsync(a => a.IngredientCode.Equals(code));
            }
            else if (!id.HasValue && code != null)
            {
                return await _context
                    .Ingredients.Include(i => i.Images)
                    .Include(i => i.Category)
                    .Include(i => i.IngredientQuantities)
                    .Include(i => i.IngredientReviews)
                    .FirstAsync(i => i.Id == id && i.IngredientCode == code);
            }
            else
            {
                return null;
            }                
        }

        public async Task<Ingredient> GetById(Guid id)
        {
            return await _context
                .Ingredients.Include(i => i.Images)
                .Include(i => i.Category)
                .Include(i => i.IngredientQuantities)
                .Include(i => i.IngredientReviews)
                .FirstAsync(a => a.Id.Equals(id));
        }

        public async Task<Ingredient> CreateAsync(Ingredient ingredient)
        {
            _context.Ingredients.Add(ingredient);
            await _context.SaveChangesAsync();
            return ingredient;
        }

        public async Task<Ingredient> UpdateAsync(Guid id, Ingredient ingredient)
        {
            var existingIngredient = await _context.Ingredients.FindAsync(id);
            if (existingIngredient == null)
            {
                throw new Exception("Ingredient not found.");
            }

            _context.Entry(existingIngredient).CurrentValues.SetValues(ingredient);
            await _context.SaveChangesAsync();
            return existingIngredient;
        }

        public async Task<bool> CheckCode(string code)
        {
            return await _context.Ingredients.AnyAsync(a => a.IngredientCode.Equals(code));
        }

        public async Task<bool> ChangeStatus(Guid id)
        {
            var existingIngredient = await GetById(id);
            if (existingIngredient != null)
            {
                existingIngredient.IngredientStatus = IngredientStatus.NO_ACTIVE;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<Ingredient>> GetIngredientsByPriceRangeAsync(double minPrice, double maxPrice)
        {
            return await _context.Ingredients
                .Where(i => i.PriceOrigin >= minPrice && i.PriceOrigin <= maxPrice)
                .ToListAsync();
        }
    }
}
