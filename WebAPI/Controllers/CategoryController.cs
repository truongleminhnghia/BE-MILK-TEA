using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Services;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryService categorySrvice, IMapper mapper)
        {
            _categoryService = categorySrvice;
            _mapper = mapper;
        }

        //GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        //GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound();
            return Ok(category);
        }

        //CREATE
        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] CategoryRequest category)
        {
            if (category == null)
            {
                return BadRequest(new { message = "Invalid category data" });
            }

            var createdCategory = await _categoryService.CreateAsync(
                _mapper.Map<Category>(category)
            );
            return Ok(createdCategory);
        }

        //UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(
            Guid id,
            [FromBody] CategoryRequest categoryRequest
        )
        {
            if (categoryRequest == null)
            {
                return BadRequest(new { message = "Invalid category data" });
            }

            var category = _mapper.Map<Category>(categoryRequest);
            var updatedCategory = await _categoryService.UpdateAsync(id, category);

            if (updatedCategory == null)
            {
                return NotFound(new { message = "Category not found" });
            }

            return Ok(updatedCategory);
        }

        //DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var result = await _categoryService.DeleteAsync(id);

            if (!result)
            {
                return NotFound(new { message = "Category not found" });
            }

            return Ok(new { message = "Category deleted successfully" });
        }
    }
}
