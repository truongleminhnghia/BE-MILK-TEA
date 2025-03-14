using Data_Access_Layer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Repositories
{
    public interface IIngredientProductRepository
    {
        Task<IngredientProduct> CreateAsync(IngredientProduct ingredientProduct);
        Task UpdateAsync(IngredientProduct ingredientProduct);
        Task<bool> IngredientExistsAsync(Guid ingredientId);

        public Task<IngredientProduct> GetIngredientProductbyId(Guid ingredientProductId);
        public Task<IngredientProduct> UpdateAsync(Guid ingredientProductId, IngredientProduct ingredientProduct);
        Task<IEnumerable<IngredientProduct>> GetAllAsync(Guid? ingredientId, int page, int pageSize);
        IQueryable<IngredientProduct> Query();

    }
}
