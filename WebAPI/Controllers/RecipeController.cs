using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Business_Logic_Layer.Services;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Net;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/recipes")]
    //[Authorize(Roles = "ROLE_STAFF" )]

    public class RecipeController : ControllerBase
    {
        private readonly IRecipeService _recipeService;
        private readonly IRedisService _redisCacheService;
        private const string RecipeCacheKey = "recipe_cache";
        private const int CacheExpirationMinutes = 10;

        public RecipeController(IRecipeService recipeService, IRedisService redisCacheService)
        {
            _recipeService = recipeService;
            _redisCacheService = redisCacheService;
        }

        [HttpPost]
        [Authorize(Roles = "ROLE_STAFF")]
        public async Task<IActionResult> CreateRecipe([FromBody] RecipeRequest request)
        {
            try
            {
                var recipe = await _recipeService.CreateRecipe(request);
                await _redisCacheService.RemoveByPrefixAsync(RecipeCacheKey);

                return Ok(new ApiResponse(
                        HttpStatusCode.OK.GetHashCode(),
                        true,
                        "Tạo công thức thành công",
                        recipe
                    ));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(HttpStatusCode.InternalServerError.GetHashCode(), false, ex.Message));
            }

        }

        [HttpGet("{id}")]
        //[Authorize(Roles = "ROLE_ADMIN, ROLE_STAFF, ROLE_MANAGER")]
        public async Task<IActionResult> GetRecipeById(Guid id)
        {
            try
            {
                // Generate a unique cache key based on all parameters
                var cacheKey = $"{RecipeCacheKey}:{id}";
                // Try to get data from cache first
                var cachedData = await _redisCacheService.GetAsync<RecipeResponse>(cacheKey);
                if (cachedData != null)
                {
                    return Ok(new ApiResponse(HttpStatusCode.OK.GetHashCode(), true, "Thành công (from cache)", cachedData));
                }
                var recipe = await _recipeService.GetRecipeById(id);
                if (recipe == null) return NotFound();
                await _redisCacheService.SetAsync(cacheKey, recipe, TimeSpan.FromMinutes(CacheExpirationMinutes));
                return Ok(new ApiResponse(
                            HttpStatusCode.OK.GetHashCode(),
                            true,
                            "Lấy công thức thành công",
                            recipe
                        ));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(HttpStatusCode.InternalServerError.GetHashCode(), false, ex.Message));
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "ROLE_STAFF")]
        public async Task<IActionResult> UpdateRecipe(Guid id, [FromBody] RecipeRequest request)
        {
            try
            {
                var isUpdated = await _recipeService.UpdateRecipe(id, request);
                if (isUpdated == null)
                    return NotFound(new { success = false, message = "Không tìm thấy công thức hoặc cập nhật thất bại!" });

                await _redisCacheService.RemoveByPrefixAsync(RecipeCacheKey);
                return Ok(new ApiResponse(
                                HttpStatusCode.OK.GetHashCode(),
                                true,
                                "Cập nhật công thức thành công!",
                                isUpdated
                            ));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(HttpStatusCode.InternalServerError.GetHashCode(), false, ex.Message));
            }
        }

        [HttpGet]
        // [Authorize(Roles = "ROLE_ADMIN, ROLE_STAFF, ROLE_MANAGER")]
        public async Task<IActionResult> GetAllRecipes(
            [FromQuery] Guid userId,
            [FromQuery] string? search,
            [FromQuery] string? sortBy,
            [FromQuery] RecipeStatusEnum? recipeStatus = null,
            [FromQuery] bool isDescending = false,
            [FromQuery] Guid? categoryId = null,
            [FromQuery] RecipeLevelEnum? recipeLevel = null,
            [FromQuery] DateOnly? startDate = null,
            [FromQuery] DateOnly? endDate = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            // Generate a unique cache key based on all parameters
            var cacheKey = $"{RecipeCacheKey}:{userId}:{search}:{sortBy}:{recipeStatus}:{isDescending}:{categoryId}:{recipeLevel}:{startDate}:{endDate}:{page}:{pageSize}";
            // Try to get data from cache first
            var cachedData = await _redisCacheService.GetAsync<PageResult<RecipeResponse>>(cacheKey);
            if (cachedData != null)
            {
                return Ok(new ApiResponse(HttpStatusCode.OK.GetHashCode(), true, "Thành công (from cache)", cachedData));
            }
            try
            {
                var recipes = await _recipeService.GetAllRecipesAsync(
             search, sortBy, isDescending, recipeStatus, categoryId, recipeLevel, startDate, endDate, page, pageSize, userId);
                await _redisCacheService.SetAsync(cacheKey, recipes, TimeSpan.FromMinutes(CacheExpirationMinutes));
                return Ok(new ApiResponse(
                    HttpStatusCode.OK.GetHashCode(),
                    true,
                    "Lấy danh sách công thức thành công",
                    recipes
                ));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(HttpStatusCode.InternalServerError.GetHashCode(), false, ex.Message));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ROLE_ADMIN, ROLE_STAFF, ROLE_MANAGER")]
        public async Task<IActionResult> DeleteRecipeStatus(Guid id, RecipeStatusEnum requestStatus)
        {
            try
            {
                var isUpdated = await _recipeService.UpdateRecipeStatusAsync(id, requestStatus);
                if (isUpdated == null)
                    return NotFound(new { success = false, message = "Không tìm thấy công thức hoặc cập nhật thất bại!" });

                await _redisCacheService.RemoveByPrefixAsync(RecipeCacheKey);
                return Ok(new ApiResponse(
                                HttpStatusCode.OK.GetHashCode(),
                                true,
                                "Cập nhật trạng thái công thức thành công!",
                                isUpdated
                            ));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(HttpStatusCode.InternalServerError.GetHashCode(), false, ex.Message));
            }
        }
    }

}
