using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;

namespace Data_Access_Layer.Repositories
{
    public interface IRecipeRepository
    {
        Task<Recipe> CreateRecipe(Recipe recipe);
        Task<Recipe?> GetRecipeById(Guid recipeId);
        Task<Recipe?> UpdateRecipe(Recipe recipe);
        Task DeleteIngredientsByRecipeIdAsync(Guid recipeId);
        Task<IEnumerable<Recipe>> GetAllRecipes(string? search, string? sortBy, bool isDescending, Guid? categoryId, int page, int pageSize);
        Task<(IEnumerable<Recipe>, int TotalCount)> GetAllRecipesAsync(
            string? search, string? sortBy, bool isDescending,
            RecipeStatusEnum? recipeStatus, Guid? categoryId,
            DateTime? startDate, DateTime? endDate,
            int page, int pageSize);

    }
}