using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Repositories
{
    public class IngredientRecipeRepository : IIngredientRecipeRepository
    {
        private readonly ApplicationDbContext _context;

        public IngredientRecipeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IngredientRecipe> CreateIngredientRecipe(IngredientRecipe ingredientRecipe)
        {
            _context.IngredientRecipes.Add(ingredientRecipe);
            await _context.SaveChangesAsync();
            return ingredientRecipe;
        }

        public async Task<IngredientRecipe> UpdateIngredientRecipe(IngredientRecipe ingredientRecipe)
        {
            _context.IngredientRecipes.Update(ingredientRecipe);
            await _context.SaveChangesAsync();
            return ingredientRecipe;
        }
    }
}
