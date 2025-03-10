using AutoMapper;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;
using Data_Access_Layer.Repositories;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Business_Logic_Layer.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<PageResult<CategoryResponse>> GetAllCategoriesAsync(
                    string? search, string? sortBy, bool isDescending,
                    CategoryStatus? categoryStatus, CategoryType? categoryType,
                    DateTime? startDate, DateTime? endDate,
                    int page, int pageSize)
        {
            var(categories, total) = await _categoryRepository.GetAllCategoriesAsync(
                search, sortBy, isDescending, categoryStatus, categoryType, startDate, endDate, page, pageSize);

            return new PageResult<CategoryResponse>
            {
                Data = _mapper.Map<List<CategoryResponse>>(categories),
                PageCurrent = page,
                PageSize = pageSize,
                Total = total
            };
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

        public async Task<List<Dictionary<string, object>>> GetField(string fieldQuery, CategoryStatus status)
        {
            try
            {
                var fields = fieldQuery?.Split(',').Select(f => f.Trim()).ToList() ?? new List<string> { "Id", "CategoryName" };
                var result = await _categoryRepository.GetBySomeField(fields, status);
                if (result == null)
                {
                    throw new Exception("Không có dữ liệu");
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: ", ex.Message);
                return null;
            }
        }
    }
}
