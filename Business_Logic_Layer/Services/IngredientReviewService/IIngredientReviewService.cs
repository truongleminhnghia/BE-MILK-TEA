using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;

namespace Business_Logic_Layer.Services.IngredientReviewService
{
    public interface IIngredientReviewService
    {
        Task<IEnumerable<IngredientReviewResponse>> GetAllAsync();
        Task<IEnumerable<IngredientReviewResponse>> GetByIngredientIdAsync(Guid ingredientId);
        Task<IEnumerable<IngredientReviewResponse>> GetByAccountIdAsync(Guid accountId);
        Task<IngredientReviewResponse> GetByIdAsync(Guid id);
        Task<IngredientReviewResponse> CreateAsync(CreateIngredientReviewRequest request);
        Task<IngredientReviewResponse> UpdateAsync(Guid id, UpdateIngredientReviewRequest request);
        Task<bool> DeleteAsync(Guid id);
    }
}
