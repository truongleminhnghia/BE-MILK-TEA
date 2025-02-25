using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using Data_Access_Layer.Entities;

namespace Business_Logic_Layer.Services.IngredientService
{
    public interface IIngredientService
    {
        Task<IEnumerable<Ingredient>> GetAllIngredientsAsync();
        Task<Ingredient> GetIngredientByIdAsync(Guid id);
        Task<Ingredient> CreateIngredientAsync(Ingredient ingredient);
        Task<Ingredient> UpdateIngredientAsync(Guid id, Ingredient ingredient);
        Task<bool> DeleteIngredientAsync(Guid id);
    }
}
