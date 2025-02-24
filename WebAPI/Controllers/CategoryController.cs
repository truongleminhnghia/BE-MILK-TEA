using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Business_Logic_Layer.Services;
using Data_Access_Layer.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
            var category = await _categoryService.GetAllCategoriesAsync();
            var categoryRes = _mapper.Map<IEnumerable<CategoryResponse>>(category);
            return Ok(new ApiResponse
                (HttpStatusCode.OK,
                true,
                "Thành công",
                categoryRes));
        }

        //GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            var categoryRes = _mapper.Map<CategoryResponse>(category);
            if (category == null) return NotFound(new ApiResponse
                                                    (HttpStatusCode.NotFound,
                                                    false,
                                                    "Không tìm thấy"));
            return Ok(new ApiResponse
                (HttpStatusCode.OK,
                true,
                "Tìm thành công",
                categoryRes));
        }

        //CREATE
        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] CategoryRequest category)
        {
            if (category == null)
            {
                return BadRequest(new ApiResponse(
                    HttpStatusCode.BadRequest,
                    false,
                    "Data không hợp lệ"));
            }
            var existingCategory = await _categoryService.GetByNameAsync(category.CategoryName);
            if (existingCategory != null)
            {
                return BadRequest(new ApiResponse(
                    HttpStatusCode.BadRequest,
                    false,
                    "Tên danh mục đã tồn tại"));
            }

            var createdCategory = await _categoryService.CreateAsync(_mapper.Map<Category>(category));
            return Ok(new ApiResponse(
                HttpStatusCode.OK,
                true,
                "Tạo thành công"
                ));
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
                return BadRequest(new ApiResponse
                    (HttpStatusCode.BadRequest,
                    false,
                    "Data không hợp lệ"));
            }

            var category = _mapper.Map<Category>(categoryRequest);
            var updatedCategory = await _categoryService.UpdateAsync(id, category);

            if (updatedCategory == null)
            {
                return NotFound(new ApiResponse
                    (HttpStatusCode.NotFound,
                    false,
                    "Không tìm thấy"));
            }

            return Ok(new ApiResponse
                (HttpStatusCode.OK,
                true,
                "Cập nhật thành công"));
        }

        //DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var result = await _categoryService.DeleteAsync(id);

            if (!result)
            {
                return NotFound(new ApiResponse
                    (HttpStatusCode.NotFound,
                    false,
                    "Không tìm thấy"));
            }

            return Ok(new ApiResponse
                (HttpStatusCode.OK,
                true,
                "Xoá thành công"));
        }
    }
}
