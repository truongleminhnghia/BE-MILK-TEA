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

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
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
                return false; 
        }

            _context.Categories.Remove(existingCategory);
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
