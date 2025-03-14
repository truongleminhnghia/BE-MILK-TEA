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

        public async Task<(IEnumerable<Category>, int TotalCount)> GetAllCategoriesAsync(
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
                DateTime adjustedEndDate = endDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(c => c.CreateAt >= startDate.Value && c.CreateAt <= adjustedEndDate);
            }
            else if (startDate.HasValue)
            {
                query = query.Where(c => c.CreateAt >= startDate.Value);
            }
            else if (endDate.HasValue)
            {
                DateTime adjustedEndDate = endDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(c => c.CreateAt <= adjustedEndDate);
                isDescending = true; // Force descending order if only endDate is provided
            }



            // **Sorting**
            if (!string.IsNullOrEmpty(sortBy))
            {
                query = isDescending
                    ? query.OrderByDescending(e => EF.Property<object>(e, sortBy))
                    : query.OrderBy(e => EF.Property<object>(e, sortBy));
            }
            else
            {
                query = query.OrderByDescending(c => c.CreateAt); // Default sorting by CreateAt descending
            }

            int total = await query.CountAsync();


            // **Pagination**
            var categories = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

            return (categories, total);
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

        public async Task<List<Dictionary<string, object>>> GetBySomeField(List<string> fields, CategoryStatus status)
        {
            var query = _context.Categories.AsQueryable();
            if (!string.IsNullOrEmpty(status.ToString()))
            {
                query = query.Where(c => status.Equals(c.CategoryStatus));
            }
            var categories = await query.ToListAsync();
            // lấy field được yêu cầu
            var result = categories.Select(category =>
            {
                var dict = new Dictionary<string, object>();
                foreach (var field in fields)
                {
                    var prop = typeof(Category).GetProperty(field, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    if (prop != null)
                    {
                        dict[field] = prop.GetValue(category);
                    }
                }
                return dict;
            }).ToList();
            return result;
        }

    }
}