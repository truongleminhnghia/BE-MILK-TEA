using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;

namespace Business_Logic_Layer.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync(
                    string? search, string? sortBy, bool isDescending,
                    int? categoryStatus, int? categoryType,
                    DateTime? startDate, DateTime? endDate,
                    int page, int pageSize)
        {
            return await _categoryRepository.GetAllCategoriesAsync(
                search, sortBy, isDescending, categoryStatus, categoryType, startDate, endDate, page, pageSize);
        }
        public async Task<Category?> GetByIdAsync(Guid id)
        {
            return await _categoryRepository.GetByIdAsync(id);
        }

        public async Task<Category> CreateAsync(Category category)
        {
            return await _categoryRepository.CreateAsync(category);
        }
        public async Task<Category?> GetByNameAsync(string name)
        {
            return await _categoryRepository.GetByNameAsync(name);
        }
        public async Task<Category?> UpdateAsync(Guid id, Category category)
        {
            return await _categoryRepository.UpdateAsync(id, category);
        }
        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _categoryRepository.DeleteAsync(id);
        }
    }
}
