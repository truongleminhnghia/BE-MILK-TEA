using Data_Access_Layer.Entities;

namespace Data_Access_Layer.Repositories
{
    public interface IRecipeRepository
    {
            Task<Recipe> CreateRecipe(Recipe recipe);
            Task<Recipe?> GetRecipeById(Guid recipeId);       

    }
}