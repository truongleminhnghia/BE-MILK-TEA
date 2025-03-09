using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;

namespace Data_Access_Layer.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync(
            string? search, string? sortBy, bool isDescending,
            CategoryStatus? categoryStatus, CategoryType? categoryType,
            DateTime? startDate, DateTime? endDate,
            int page, int pageSize);
        Task<Category?> GetByIdAsync(Guid id);
        Task<Category> CreateAsync(Category category);
        Task<Category?> GetByNameAsync(string name);
        Task<Category?> UpdateAsync(Guid id, Category category);
        Task<bool> DeleteAsync(Guid id);
        public Task<List<Dictionary<string, object>>> GetBySomeField(List<string> fields, CategoryStatus status);
    }
}
