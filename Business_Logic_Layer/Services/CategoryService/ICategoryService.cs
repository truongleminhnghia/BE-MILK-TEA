using Data_Access_Layer.Entities;
using Business_Logic_Layer.Models.Requests;

namespace Business_Logic_Layer.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync(
            string? search, string? sortBy, bool isDescending,
            int? categoryStatus, int? categoryType,
            DateTime? startDate, DateTime? endDate,
            int page, int pageSize);
        Task<Category?> GetByIdAsync(Guid id);
        Task<Category> CreateAsync(Category category);
        Task<Category?> GetByNameAsync(string name);
        Task<Category?> UpdateAsync(Guid id, Category category);
        Task<bool> DeleteAsync(Guid id);
    }
}
