using Data_Access_Layer.Entities;

namespace Data_Access_Layer.Repositories
{
    public interface IRecipeRepository
    {
        Task<Recipe> CreateRecipe(Recipe recipe);
        Task<Recipe?> GetRecipeById(Guid recipeId);
        Task<Recipe?> UpdateRecipe(Recipe recipe);
        Task DeleteIngredientsByRecipeIdAsync(Guid recipeId);
        Task<IEnumerable<Recipe>> GetAllRecipes(string? search, string? sortBy, bool isDescending, Guid? categoryId, int page, int pageSize);

    }
}