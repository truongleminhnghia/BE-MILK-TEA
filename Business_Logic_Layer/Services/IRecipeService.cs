using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;

namespace Business_Logic_Layer.Services
{
    public interface IRecipeService
    {
        Task<RecipeResponse> CreateRecipe(RecipeRequest request);
        Task<RecipeResponse?> GetRecipeById(Guid recipeId);
        Task<RecipeResponse?> UpdateRecipe(Guid recipeId, RecipeRequest request);        
        Task<PageResult<RecipeResponse>> GetAllRecipesAsync(
            string? search, string? sortBy, bool isDescending,
            RecipeStatusEnum? recipeStatus, Guid? categoryId,
            DateTime? startDate, DateTime? endDate,
            int page, int pageSize);

    }
}