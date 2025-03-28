using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
                .Include(r => r.Category)
                .Include(r => r.IngredientRecipes)
                    .ThenInclude(ir => ir.Ingredient)
                        .ThenInclude(i => i.Category)
                 .Include(r => r.IngredientRecipes)
                    .ThenInclude(ir => ir.Ingredient)
                        .ThenInclude(i => i.Images)
                 .Include(r => r.IngredientRecipes)
                    .ThenInclude(ir => ir.Ingredient)
                        .ThenInclude(i => i.IngredientQuantities)
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

        public async Task<List<Recipe>> GetAllRecipes()
        {
            return await _context.Recipes
                .Include(r => r.Category)
                .Include(r => r.IngredientRecipes)
                    .ThenInclude(ir => ir.Ingredient)
                        .ThenInclude(i => i.Category)
                 .Include(r => r.IngredientRecipes)
                    .ThenInclude(ir => ir.Ingredient)
                        .ThenInclude(i => i.Images)
                 .Include(r => r.IngredientRecipes)
                    .ThenInclude(ir => ir.Ingredient)
                        .ThenInclude(i => i.IngredientQuantities)
                .ToListAsync();
        }


        public async Task<(List<Recipe>, int)> GetAllRecipesAsync(
    string? search, string? sortBy, bool isDescending,
    RecipeStatusEnum? recipeStatus, Guid? categoryId, RecipeLevelEnum? recipeLevel,
    DateOnly? startDate, DateOnly? endDate,
    int page, int pageSize)
        {
            var query = _context.Recipes
                .Include(r => r.Category)
                .Include(r => r.IngredientRecipes)
                    .ThenInclude(ir => ir.Ingredient)
                        .ThenInclude(i => i.Category)
                 .Include(r => r.IngredientRecipes)
                    .ThenInclude(ir => ir.Ingredient)
                        .ThenInclude(i => i.Images)
                 .Include(r => r.IngredientRecipes)
                    .ThenInclude(ir => ir.Ingredient)
                        .ThenInclude(i => i.IngredientQuantities)
                .AsQueryable();

            // Apply Filters if not null
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(r => r.RecipeTitle.Contains(search) || r.Content.Contains(search));
            }

            if (recipeStatus.HasValue)
            {
                query = query.Where(r => r.RecipeStatus == recipeStatus.Value);
            }

            if (categoryId.HasValue)
            {
                query = query.Where(r => r.CategoryId == categoryId.Value);
            }

            if (startDate.HasValue || endDate.HasValue)
            {
                DateTime adjustedStart = startDate?.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue;
                DateTime adjustedEnd = endDate?.ToDateTime(TimeOnly.MaxValue) ?? DateTime.MaxValue;

                query = query.Where(i => i.CreateAt >= adjustedStart && i.CreateAt <= adjustedEnd);
            }

            if (recipeLevel.HasValue)
            {
                query = query.Where(r => r.RecipeLevel == recipeLevel.Value);
            }

            // Sorting
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                query = isDescending
                    ? query.OrderByDescending(r => EF.Property<object>(r, sortBy))
                    : query.OrderBy(r => EF.Property<object>(r, sortBy));
            }
            else
            {
                query = query.OrderByDescending(r => r.CreateAt); // Default sort by latest created
            }

            // Get Total Count Before Pagination
            int total = await query.CountAsync();

            // Apply Pagination
            var recipes = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (recipes, total);
        }

        public async Task<List<Recipe>> GetFilteredRecipesAsync(
            string? search, string? sortBy, bool isDescending,
            RecipeStatusEnum? recipeStatus, Guid? categoryId, RecipeLevelEnum? recipeLevel,
            DateOnly? startDate, DateOnly? endDate)
        {
            var query = _context.Recipes
                .Include(r => r.Category)
                .Include(r => r.IngredientRecipes)
                    .ThenInclude(ir => ir.Ingredient)
                        .ThenInclude(i => i.Category)
                 .Include(r => r.IngredientRecipes)
                    .ThenInclude(ir => ir.Ingredient)
                        .ThenInclude(i => i.Images)
                 .Include(r => r.IngredientRecipes)
                    .ThenInclude(ir => ir.Ingredient)
                        .ThenInclude(i => i.IngredientQuantities)
                .AsQueryable();

            // Lọc theo RecipeLevel nếu có truyền vào
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(r => r.RecipeTitle.Contains(search) || r.Content.Contains(search));
            }

            if (recipeStatus.HasValue)
            {
                query = query.Where(r => r.RecipeStatus == recipeStatus.Value);
            }

            if (categoryId.HasValue)
            {
                query = query.Where(r => r.CategoryId == categoryId.Value);
            }

            if (startDate.HasValue || endDate.HasValue)
            {
                DateTime adjustedStart = startDate?.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue;
                DateTime adjustedEnd = endDate?.ToDateTime(TimeOnly.MaxValue) ?? DateTime.MaxValue;

                query = query.Where(i => i.CreateAt >= adjustedStart && i.CreateAt <= adjustedEnd);
            }


            if (recipeLevel.HasValue)
            {
                query = query.Where(r => r.RecipeLevel == recipeLevel.Value);
            }

            // Sorting
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                query = isDescending
                    ? query.OrderByDescending(r => EF.Property<object>(r, sortBy))
                    : query.OrderBy(r => EF.Property<object>(r, sortBy));
            }
            else
            {
                query = query.OrderByDescending(r => r.CreateAt); // Default sort by latest created
            }
            if (startDate.HasValue || endDate.HasValue)
            {
                DateTime adjustedStart = startDate?.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue;
                DateTime adjustedEnd = endDate?.ToDateTime(TimeOnly.MaxValue) ?? DateTime.MaxValue;

                query = query.Where(i => i.CreateAt >= adjustedStart && i.CreateAt <= adjustedEnd);
            }

            // Trả về danh sách chưa phân trang
            return await query.ToListAsync();
        }


        public async Task<Recipe?> GetByTitleAsync(string title)
        {
            return await _context.Recipes
                .FirstOrDefaultAsync(r => r.RecipeTitle.Trim().ToLower() == title.Trim().ToLower());
        }


    }

}
