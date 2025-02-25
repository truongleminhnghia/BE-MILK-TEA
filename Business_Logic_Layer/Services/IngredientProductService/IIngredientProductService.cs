using Data_Access_Layer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Services.IngredientProductService
{
    public interface IIngredientProductService
    {
        Task<IngredientProduct> CreateAsync(IngredientProduct ingredientProduct);
        Task<bool> IngredientExistsAsync(Guid ingredientId);
    }
}
