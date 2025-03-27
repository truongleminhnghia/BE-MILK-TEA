using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;

namespace Business_Logic_Layer.Services.IngredientService
{
    public interface IIngredientService
    {
        // Task<IEnumerable<Ingredient>> GetAllIngredientsAsync(string? search, Guid? categoryId, string? sortBy, bool isDescending, int page, int pageSize, DateTime? startDate, DateTime? endDate, IngredientStatus? status);
        public Task<PageResult<IngredientResponse>> GetAllAsync(
        string? search,
        string? categorySearch,
        Guid? categoryId,
        string? sortBy,
        bool isDescending,
        int pageCurrent,
        int pageSize,
        DateOnly? startDate,
        DateOnly? endDate,
        IngredientStatus? status,
        decimal? minPrice,
        decimal? maxPrice,
        bool? isSale);
        Task<IngredientResponse> GetById(Guid id);
        Task<IngredientResponse> CreateIngredientAsync(IngredientRequest request);
        Task<IngredientResponse> Update(Guid id, UpdateIngredientRequest request);
        Task<bool> ChangeStatus(Guid id);
    }
}
