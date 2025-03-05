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


    }

}
