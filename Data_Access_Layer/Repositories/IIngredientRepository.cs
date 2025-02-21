using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer.Entities;

namespace Data_Access_Layer.Repositories
{
    public interface IIngredientRepository
    {
        Task<IEnumerable<Ingredient>> GetAllAsync();
        Task<Ingredient> GetByIdAsync(Guid id);
        Task<Ingredient> CreateAsync(Ingredient ingredient);
        Task<Ingredient> UpdateAsync(Ingredient ingredient);
        Task<bool> DeleteAsync(Guid id);
        Task<Ingredient> GetLastIngredientCode();
        Task<bool> CategoryExistsAsync(Guid categoryId);
        Task GetIngredientByIdAsync(Guid ingredientId);
    }
}
