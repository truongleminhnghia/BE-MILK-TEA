using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer.Entities;

namespace Data_Access_Layer.Repositories
{
    public interface IIngredientReviewRepository
    {
        Task<IEnumerable<IngredientReview>> GetAllAsync();
        Task<IEnumerable<IngredientReview>> GetByIngredientIdAsync(Guid ingredientId);
        Task<IEnumerable<IngredientReview>> GetByAccountIdAsync(Guid accountId);
        Task<IngredientReview> GetByIdAsync(Guid id);
        Task<IngredientReview> CreateAsync(IngredientReview ingredientReview);
        Task<IngredientReview> UpdateAsync(IngredientReview ingredientReview);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> ExistsReviewByAccountAndIngredientAsync(Guid accountId, Guid ingredientId);
    }
}
