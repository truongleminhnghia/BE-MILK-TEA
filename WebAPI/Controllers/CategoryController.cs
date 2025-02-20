using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Services;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            var categoriesDto = _mapper.Map<IEnumerable<CategoryRequest>>(categories);
            return Ok(categoriesDto);
        }
        //GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null) return NotFound();

            var categoryDto = _mapper.Map<CategoryRequest>(category);
            return Ok(categoryDto);
        }

        //CREATE
        [HttpPost]
        public async Task<IActionResult> Create(Category categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);
            var createdCategory = await _categoryRepository.AddAsync(category);
            var createdCategoryDto = _mapper.Map<CategoryDTO>(createdCategory);

            return CreatedAtAction(nameof(GetById), new { id = createdCategoryDto.Id }, createdCategoryDto);
        }
    }
}
