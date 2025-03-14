using Data_Access_Layer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Repositories
{
    public interface IIngredientRecipeRepository
    {
        Task<IngredientRecipe> CreateIngredientRecipe(IngredientRecipe ingredientRecipe);
    }
}
