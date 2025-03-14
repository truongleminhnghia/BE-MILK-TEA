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
        Task<List<IngredientQuantityResponse>> GetByIngredientId(Guid ingredientId);
        Task<IngredientQuantityResponse> CreateAsync(IngredientQuantityRequest request);
        Task<IngredientQuantityResponse> UpdateAsync(Guid id, IngredientQuantityRequest request);

        Task<IngredientQuantityResponse> GetByIdAndProductType(Guid ingredientId, ProductType ProductType);


        Task<List<IngredientQuantityResponse>> CreateQuantitiesAsync(Guid ingredientId, List<IngredientQuantityRequest> request);
        Task<List<IngredientQuantityResponse>> UpdateQuantitiesAsync(Guid ingredientId, List<IngredientQuantityRequest> request);
        Task<IngredientQuantity> Save(IngredientQuantity ingredientQuantity);
        Task<IngredientQuantity> SaveList(Guid ingredientId, List<IngredientQuantity> ingredientQuantity);

    }
}
