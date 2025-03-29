using Data_Access_Layer.Entities;
using Business_Logic_Layer.Models.Requests;
using Data_Access_Layer.Enum;
using Business_Logic_Layer.Models.Responses;

namespace Business_Logic_Layer.Services
{
    public interface ICategoryService
    {
        Task<PageResult<CategoryResponse>> GetAllCategoriesAsync(
            string? search, string? sortBy, bool isDescending,
            CategoryStatus? categoryStatus, CategoryType? categoryType,
            DateTime? startDate, DateTime? endDate,
            int page, int pageSize);
        Task<Category?> GetByIdAsync(Guid id);
        Task<Category> CreateAsync(Category category);
        Task<Category?> GetByNameAsync(string name);
        Task<Category?> UpdateAsync(Guid id, Category category);
        Task<bool> DeleteAsync(Guid id);
        public Task<List<Dictionary<string, object>>> GetField(string fieldQuery, CategoryStatus status, CategoryType? type = null);
    }
}
