using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;
using Microsoft.EntityFrameworkCore;

namespace Data_Access_Layer.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync(
            string? search, string? sortBy, bool isDescending,
            CategoryStatus? categoryStatus, CategoryType? categoryType,
            DateTime? startDate, DateTime? endDate,
            int page, int pageSize)
        {
            var query = _context.Categories.AsQueryable();

            // **Filtering by name**
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.CategoryName.Contains(search));
            }

            // **Filtering by CategoryStatus**
            if (categoryStatus.HasValue)
            {
                query = query.Where(c => c.CategoryStatus == categoryStatus.Value);
            }

            // **Filtering by CategoryType**
            if (categoryType.HasValue)
            {
                query = query.Where(c => c.CategoryType == categoryType.Value);
            }

            // **Filtering by date range (CreateAt)**
            if (startDate.HasValue && endDate.HasValue)
            {
                DateTime adjustedEndDate = endDate.Value.Date.AddDays(1).AddTicks(-1); // Includes full day

                query = query.Where(c => c.CreateAt >= startDate.Value && c.CreateAt <= adjustedEndDate);
            }


            // **Sorting**
            if (!string.IsNullOrEmpty(sortBy))
            {
                query = isDescending
                    ? query.OrderByDescending(e => EF.Property<object>(e, sortBy))
                    : query.OrderBy(e => EF.Property<object>(e, sortBy));
            }
            

            // **Pagination**
            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            return await query.ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(Guid id)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<Category?> GetByNameAsync(string name)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.CategoryName == name);
        }


        public async Task<Category> CreateAsync(Category category)
        {
            category.CreateAt = DateTime.Now;
            category.UpdateAt = DateTime.Now;

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category?> UpdateAsync(Guid id, Category category)
        {
            var existingCategory = await _context.Categories.FindAsync(id);
            if (existingCategory == null)
        {
                return null;
        }

            existingCategory.CategoryName = category.CategoryName;
            existingCategory.CategoryStatus = category.CategoryStatus;
            existingCategory.CategoryType = category.CategoryType;

            existingCategory.UpdateAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return existingCategory;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existingCategory = await _context.Categories.FindAsync(id);
            if (existingCategory == null)
            {
                throw new KeyNotFoundException("Không tìm thấy.");
            }

            if (existingCategory.CategoryStatus == CategoryStatus.NO_ACTIVE)
            {
                throw new InvalidOperationException("Danh mục đang không hoạt động.");
            }
            bool hasIngredients = await _context.Ingredients.AnyAsync(i => i.CategoryId == id);
            if (hasIngredients)
            {
                throw new InvalidOperationException("Không thể tắt danh mục vì có nguyên liệu liên quan.");
            }
            existingCategory.CategoryStatus = CategoryStatus.NO_ACTIVE;

            await _context.SaveChangesAsync();

            return true;
        }

    }
}



//using AutoMapper;
//using Business_Logic_Layer.Services;
//using Business_Logic_Layer.Services.IngredientService;
//using Data_Access_Layer.Entities;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace WebAPI.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class CategoryController : ControllerBase
//    {
//        private readonly ICategoryService _categorytService;
//        private readonly IMapper _mapper;

//        public CategoryController(ICategoryService categorytService, IMapper mapper)
//        {
//            _categorytService = categorytService;
//            _mapper = mapper;
//        }

//        [HttpPost]
//        public async Task<IActionResult> Add([FromBody] Category ingredient)
//        {
//            if (ingredient == null)
//            {
//                return BadRequest(new { message = "Invalid ingredient data" });
//            }

//            var createdIngredient = await _categorytService.CreateAsync(ingredient);
//            return Ok(ingredient);
//        }
//    }
//}
