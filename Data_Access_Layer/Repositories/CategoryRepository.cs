using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
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

        public Task<bool> CategoryExistsAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<Category> CreateAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Category>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Category> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Category> UpdateAsync(Category category)
        {
            throw new NotImplementedException();
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
