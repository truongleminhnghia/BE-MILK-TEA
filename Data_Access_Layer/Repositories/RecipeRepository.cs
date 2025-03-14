using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Repositories
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly ApplicationDbContext _context;

        public RecipeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Recipe> CreateRecipe(Recipe recipe)
        {
            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();
            return recipe;
        }

        public async Task<Recipe?> GetRecipeById(Guid recipeId)
        {
            return await _context.Recipes
                .Include(r => r.IngredientRecipes)
                .ThenInclude(ir => ir.Ingredient)
                .FirstOrDefaultAsync(r => r.Id == recipeId);
        }

        public async Task<Recipe?> UpdateRecipe(Recipe recipe)
        {
            _context.Recipes.Update(recipe);
            await _context.SaveChangesAsync();
            return recipe;
        }

        public async Task DeleteIngredientsByRecipeIdAsync(Guid recipeId)
        {
            var ingredientRecipes = _context.IngredientRecipes
                .Where(ir => ir.RecipeId == recipeId);

            _context.IngredientRecipes.RemoveRange(ingredientRecipes);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Recipe>> GetAllRecipes(
    string? search, string? sortBy, bool isDescending,
    Guid? categoryId, int page, int pageSize)
        {
            var query = _context.Recipes
                .Include(r => r.Category)
                .Include(r => r.IngredientRecipes)
                    .ThenInclude(ir => ir.Ingredient)
                .AsQueryable();

            // **Tìm kiếm theo tiêu đề hoặc nội dung**
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(r => r.RecipeTitle.Contains(search) || r.Content.Contains(search));
            }

            // **Lọc theo danh mục**
            if (categoryId.HasValue)
            {
                query = query.Where(r => r.CategoryId == categoryId.Value);
            }

            // **Sắp xếp**
            if (!string.IsNullOrEmpty(sortBy))
            {
                query = isDescending
                    ? query.OrderByDescending(e => EF.Property<object>(e, sortBy))
                    : query.OrderBy(e => EF.Property<object>(e, sortBy));
            }

            // **Phân trang**
            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            return await query.ToListAsync();
        }

        public async Task<(IEnumerable<Recipe>, int TotalCount)> GetAllRecipesAsync(
    string? search, string? sortBy, bool isDescending,
    RecipeStatusEnum? recipeStatus, Guid? categoryId,
    DateTime? startDate, DateTime? endDate,
    int page, int pageSize)
        {
            var query = _context.Recipes.Include(r => r.Category).AsQueryable();

            // **Filtering by RecipeTitle**
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(r => r.RecipeTitle.Contains(search));
            }

            // **Filtering by RecipeStatus**
            if (recipeStatus.HasValue)
            {
                query = query.Where(r => r.RecipeStatus == recipeStatus.Value);
            }

            // **Filtering by CategoryId**
            if (categoryId.HasValue)
            {
                query = query.Where(r => r.CategoryId == categoryId.Value);
            }

            // **Filtering by date range (CreateAt)**
            if (startDate.HasValue && endDate.HasValue)
            {
                DateTime adjustedEndDate = endDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(r => r.CreateAt >= startDate.Value && r.CreateAt <= adjustedEndDate);
            }
            else if (startDate.HasValue)
            {
                query = query.Where(r => r.CreateAt >= startDate.Value);
            }
            else if (endDate.HasValue)
            {
                DateTime adjustedEndDate = endDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(r => r.CreateAt <= adjustedEndDate);
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
                query = query.OrderByDescending(r => r.CreateAt); // Default sorting by CreateAt descending
            }

            int total = await query.CountAsync();

            // **Pagination**
            var recipes = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

            return (recipes, total);
        }


    }

}
