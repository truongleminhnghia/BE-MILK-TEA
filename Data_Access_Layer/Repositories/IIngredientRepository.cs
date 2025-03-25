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
        public IQueryable<Ingredient> GetAll(
        string? search,
        string? categorySearch,
        Guid? categoryId,
        DateOnly? startDate,
        DateOnly? endDate,
        IngredientStatus? status,
        decimal? minPrice,
        decimal? maxPrice,
        bool? isSale);
        Task<Ingredient> GetById(Guid id);
        Task<Ingredient> CreateAsync(Ingredient ingredient);
        Task<Ingredient> UpdateAsync(Guid id, Ingredient ingredient);
        Task<bool> CheckCode(string code);
        Task<bool> ChangeStatus(Guid id);
    }
}
