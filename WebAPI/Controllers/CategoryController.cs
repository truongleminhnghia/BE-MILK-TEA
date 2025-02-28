using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Business_Logic_Layer.Services;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;
using Microsoft.AspNetCore.Authorization;
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
        public async Task<IActionResult> GetAll(
            [FromQuery] CategoryStatus? categoryStatus,
            [FromQuery] CategoryType? categoryType,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool isDescending = false,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null
        )
        {
            var categories = await _categoryService.GetAllCategoriesAsync(
                search,
                sortBy,
                isDescending,
                categoryStatus,
                categoryType,
                startDate,
                endDate,
                page,
                pageSize
            );

            var categoryRes = _mapper.Map<IEnumerable<CategoryResponse>>(categories);
            return Ok(
                new ApiResponse(HttpStatusCode.OK.GetHashCode(), true, "Thành công", categoryRes)
            );
        }

        //GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            var categoryRes = _mapper.Map<CategoryResponse>(category);
            if (category == null)
                return NotFound(
                    new ApiResponse(HttpStatusCode.NotFound.GetHashCode(), false, "Không tìm thấy")
                );
            return Ok(
                new ApiResponse(
                    HttpStatusCode.OK.GetHashCode(),
                    true,
                    "Tìm thành công",
                    categoryRes
                )
            );
        }

        //CREATE
        [HttpPost]
        //[Authorize(Roles = "ROLE_STAFF")]
        public async Task<IActionResult> AddCategory([FromBody] CategoryRequest category)
        {
            if (category == null)
            {
                return BadRequest(
                    new ApiResponse(
                        HttpStatusCode.BadRequest.GetHashCode(),
                        false,
                        "Data không hợp lệ"
                    )
                );
            }
            var existingCategory = await _categoryService.GetByNameAsync(category.CategoryName);
            if (existingCategory != null)
            {
                return BadRequest(
                    new ApiResponse(
                        HttpStatusCode.BadRequest.GetHashCode(),
                        false,
                        "Tên danh mục đã tồn tại"
                    )
                );
            }

            var createdCategory = await _categoryService.CreateAsync(
                _mapper.Map<Category>(category)
            );
            return Ok(new ApiResponse(HttpStatusCode.OK.GetHashCode(), true, "Tạo thành công"));
        }

        //UPDATE
        [HttpPut("{id}")]
        [Authorize(Roles = "ROLE_STAFF")]
        public async Task<IActionResult> UpdateCategory(
            Guid id,
            [FromBody] CategoryRequest categoryRequest
        )
        {
            if (categoryRequest == null)
            {
                return BadRequest(
                    new ApiResponse(
                        HttpStatusCode.BadRequest.GetHashCode(),
                        false,
                        "Data không hợp lệ"
                    )
                );
            }

            var category = _mapper.Map<Category>(categoryRequest);
            var updatedCategory = await _categoryService.UpdateAsync(id, category);

            if (updatedCategory == null)
            {
                return NotFound(
                    new ApiResponse(HttpStatusCode.NotFound.GetHashCode(), false, "Không tìm thấy")
                );
            }

            return Ok(
                new ApiResponse(HttpStatusCode.OK.GetHashCode(), true, "Cập nhật thành công")
            );
        }

        //DELETE
        [HttpDelete("{id}")]
        [Authorize(Roles = "ROLE_STAFF")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            try
            {
                var result = await _categoryService.DeleteAsync(id);

                if (!result)
                {
                    return NotFound(
                        new ApiResponse(
                            HttpStatusCode.NotFound.GetHashCode(),
                            false,
                            "Không tìm thấy"
                        )
                    );
                }

                return Ok(new ApiResponse(HttpStatusCode.OK.GetHashCode(), true, "Tắt thành công"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(
                    new ApiResponse(HttpStatusCode.NotFound.GetHashCode(), false, ex.Message)
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(
                    new ApiResponse(HttpStatusCode.BadRequest.GetHashCode(), false, ex.Message)
                );
            }
        }
    }
}
