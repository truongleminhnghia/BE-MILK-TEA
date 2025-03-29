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
using XAct.Messages;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        private readonly IRedisService _redisCacheService;
        private const string CategoriesCacheKey = "categories_cache";
        private const int CacheExpirationMinutes = 10;

        public CategoryController(ICategoryService categorySrvice, IMapper mapper, IRedisService redisCacheService)
        {
            _categoryService = categorySrvice;
            _mapper = mapper;
            _redisCacheService = redisCacheService;
        }

        //GET ALL (with Redis cache)
        [HttpGet]
        [Authorize(Roles = "ROLE_ADMIN, ROLE_STAFF, ROLE_MANAGER")]        
        
        
        
        
        public async Task<IActionResult> GetAll(
            [FromQuery] CategoryStatus? categoryStatus,
            [FromQuery] CategoryType? categoryType,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool isDescending = false,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? _field = null
        )
        {
            

            if (string.IsNullOrEmpty(_field))
            {
                // Generate a unique cache key based on all parameters
                var cacheKey = $"{CategoriesCacheKey}:{categoryStatus}:{categoryType}:{page}:{pageSize}:{search}:{sortBy}:{isDescending}:{startDate}:{endDate}";
                // Try to get data from cache first
                var cachedData = await _redisCacheService.GetAsync<PagedResponse<CategoryResponse>>(cacheKey);
                if (cachedData != null)
                {
                    return Ok(new ApiResponse(HttpStatusCode.OK.GetHashCode(), true, "Thành công (from cache)", cachedData));
                }

                // If not in cache, get from database
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
                if (categories == null || !categories.Data.Any())
                {
                    return BadRequest(new ApiResponse(HttpStatusCode.NotFound.GetHashCode(), false, "Không tìm thấy"));
                }
                // var categoryRes = _mapper.Map<List<CategoryResponse>>(categories);
                // Cache the data for future requests
                await _redisCacheService.SetAsync(cacheKey, categories, TimeSpan.FromMinutes(CacheExpirationMinutes));
                return Ok(new ApiResponse(HttpStatusCode.OK.GetHashCode(), true, "Thành công", categories));
            }
            else
            {
                var cacheKey = $"{CategoriesCacheKey}:fields:{_field.ToLower()}:{CategoryStatus.ACTIVE}";

                // Try to get from cache first
                var cachedCategories = await _redisCacheService.GetAsync<List<object>>(cacheKey);
                if (cachedCategories != null)
                {
                    return Ok(new ApiResponse(
                        HttpStatusCode.OK.GetHashCode(),
                        true,
                        "Thành công (from cache)",
                        cachedCategories
                    ));
                }
                // If not in cache, get from database
                var categories = await _categoryService.GetField(_field, CategoryStatus.ACTIVE);
                if (categories == null || !categories.Any())
                {
                    return NotFound(new ApiResponse(  // Changed from BadRequest to NotFound
                        HttpStatusCode.NotFound.GetHashCode(),
                        false,
                        "Không tìm thấy dữ liệu"
                    ));
                }
                // Store in cache for future requests
                await _redisCacheService.SetAsync(
                    cacheKey,
                    categories,
                    TimeSpan.FromMinutes(CacheExpirationMinutes));

                return Ok(new ApiResponse(
                    HttpStatusCode.OK.GetHashCode(),
                    true,
                    "Thành công",
                    categories
                ));
            }
        }

        //GET BY ID
        [HttpGet("{id}")]
        [Authorize(Roles = "ROLE_STAFF, ROLE_ADMIN, ROLE_MANAGER")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var cacheKey = $"{CategoriesCacheKey}:{id}";
            var cachedCategory = await _redisCacheService.GetAsync<CategoryResponse>(cacheKey);

            if (cachedCategory != null)
            {
                return Ok(new ApiResponse(HttpStatusCode.OK.GetHashCode(), true, "Thành công (from cache)", cachedCategory));
            }
            var category = await _categoryService.GetByIdAsync(id);
            var categoryRes = _mapper.Map<CategoryResponse>(category);
            // Cache the individual category
            await _redisCacheService.SetAsync(cacheKey, categoryRes, TimeSpan.FromMinutes(CacheExpirationMinutes));
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
        [Authorize(Roles = "ROLE_STAFF")]
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
            // Invalidate the cache for all categories since we've added a new one
            await _redisCacheService.RemoveByPrefixAsync(CategoriesCacheKey);
            return Ok(new ApiResponse(HttpStatusCode.OK.GetHashCode(), true, "Tạo thành công"));
        }

        //UPDATE
        [HttpPut("{id}")]
        [Authorize(Roles = "ROLE_STAFF")]
        public async Task<IActionResult> UpdateCategory(
            Guid id,
            [FromBody] CategoryUpdateRequest categoryRequest
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
            await _redisCacheService.RemoveAsync($"{CategoriesCacheKey}:{id}");
            await _redisCacheService.RemoveByPrefixAsync(CategoriesCacheKey);
            return Ok(
                new ApiResponse(HttpStatusCode.OK.GetHashCode(), true, "Cập nhật thành công")
            );
        }

        //DELETE
        [HttpDelete("{id}")]
        [Authorize(Roles = "ROLE_ADMIN, ROLE_STAFF, ROLE_MANAGER")]
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
