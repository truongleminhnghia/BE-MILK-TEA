using Azure.Core;
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
        Task<List<Recipe>> GetAllRecipes();
        Task<(List<Recipe>, int)> GetAllRecipesAsync(
    string? search, string? sortBy, bool isDescending,
    RecipeStatusEnum? recipeStatus, Guid? categoryId, RecipeLevelEnum? recipeLevel,
    DateTime? startDate, DateTime? endDate,
    int page, int pageSize);
        Task<Recipe?> GetByTitleAsync(string title);
    }
}