using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Services.IngredientService
{
    public interface IIngredientQuantityService
    {
        Task<PageResult<IngredientQuantityResponse>> GetAllAsync(
            string? search,
            Guid? ingredientId,
            ProductType? productType,
            int? minQuantity,
            int? maxQuantity,
            DateTime? startDate,
            DateTime? endDate,
            string? sortBy,
            bool isDescending,
            int pageCurrent,
            int pageSize
        );

        Task<List<IngredientQuantityResponse>> GetByIngredientId(Guid ingredientId);
        Task<IngredientQuantityResponse> CreateAsync(IngredientQuantityRequest request);
        Task<IngredientQuantityResponse> UpdateAsync(Guid id, IngredientQuantityRequest request);

    }
}
