using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;

namespace Business_Logic_Layer.Services.IngredientService
{
    public interface IIngredientService
    {
        Task<IEnumerable<Ingredient>> GetAllIngredientsAsync(string? search, Guid? categoryId, string? sortBy, bool isDescending, int page, int pageSize, DateTime? startDate, DateTime? endDate, IngredientStatus? status);
        Task<Ingredient> GetIngredientByIdAsync(Guid id);
        Task<Ingredient> CreateIngredientAsync(Ingredient ingredient);
        Task<Ingredient> UpdateIngredientAsync(Guid id, Ingredient ingredient);
        Task<bool> DeleteIngredientAsync(Guid id);
    }
}
