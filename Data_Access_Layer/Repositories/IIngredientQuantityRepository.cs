using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Repositories
{
    public interface IIngredientQuantityRepository
    {
        IQueryable<IngredientQuantity> GetAll(
            string? search,
            Guid? ingredientId,
            ProductType? productType,
            int? minQuantity,
            int? maxQuantity,
            DateTime? startDate,
            DateTime? endDate
        );
        Task<List<IngredientQuantity>> GetByIngredientId(Guid ingredientId);
        Task<IngredientQuantity> AddAsync(IngredientQuantity entity);
        Task UpdateAsync(IngredientQuantity entity);
        Task<IngredientQuantity> GetById(Guid id);
    }

}
