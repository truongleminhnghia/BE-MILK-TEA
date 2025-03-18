using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
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
        Task<IngredientProductResponse> CreateAsync(IngredientProductRequest request, bool isCart);
        Task<IngredientProductResponse> GetIngredientProductbyId(Guid ingredientProductId);
        Task<IngredientProductResponse> UpdateAsync(Guid id, IngredientProductRequest request, bool isCart);
        Task<bool> IngredientExistsAsync(Guid ingredientId);
        Task<IEnumerable<IngredientProduct>> GetAllAsync(Guid? ingredientId, int page, int pageSize);
        Task<bool> DeleteAsync(Guid id);

        Task<List<IngredientProduct>> GetByIngredientIdAsync(Guid ingredientId);
    }
}
