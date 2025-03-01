using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;

namespace Data_Access_Layer.Repositories
{
    public interface IIngredientRepository
    {
        Task<IEnumerable<Ingredient>> GetAllAsync(string? search, Guid? categoryId, string? sortBy, bool isDescending, int page, int pageSize, DateTime? startDate, DateTime? endDate, IngredientStatus? status);
        Task<Ingredient> GetByIdAsync(Guid id);
        Task<Ingredient> CreateAsync(Ingredient ingredient);
        Task<Ingredient> UpdateAsync(Ingredient ingredient);
        Task<bool> DeleteAsync(Guid id);
        Task<Ingredient> GetLastIngredientCode();
        Task<bool> CategoryExistsAsync(Guid categoryId);
        Task GetIngredientByIdAsync(Guid ingredientId);
    }
}
