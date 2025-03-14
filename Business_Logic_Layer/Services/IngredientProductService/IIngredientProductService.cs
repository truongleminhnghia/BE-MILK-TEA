using Business_Logic_Layer.Models.Requests;
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
        Task<IngredientProduct> CreateAsync(IngredientProductRequest request);
        Task<bool> IngredientExistsAsync(Guid ingredientId);
        Task<IngredientProduct> GetIngredientProductbyId(Guid ingredientProductId);
        public Task<IngredientProduct> UpdateAsync(Guid ingredientProductId, IngredientProductRequest request);
        Task<IEnumerable<IngredientProduct>> GetAllAsync(Guid? ingredientId, int page, int pageSize);

    }
}
