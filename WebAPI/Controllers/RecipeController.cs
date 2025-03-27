using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Business_Logic_Layer.Services;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/recipes")]
    //[Authorize(Roles = "ROLE_STAFF" )]

    public class RecipeController : ControllerBase
    {
        private readonly IRecipeService _recipeService;

        public RecipeController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        [HttpPost]
        [Authorize("ROLE_STAFF")]
        public async Task<IActionResult> CreateRecipe([FromBody] RecipeRequest request)
        {
            try
            {
                var recipe = await _recipeService.CreateRecipe(request);
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
        [Authorize("ROLE_STAFF")]
        public async Task<IActionResult> GetRecipeById(Guid id)
        {
            try
            {
                var recipe = await _recipeService.GetRecipeById(id);
                if (recipe == null) return NotFound();

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
        [Authorize("ROLE_STAFF")]
        public async Task<IActionResult> UpdateRecipe(Guid id, [FromBody] RecipeRequest request)
        {
            try
            {
                var isUpdated = await _recipeService.UpdateRecipe(id, request);
                if (isUpdated == null)
                    return NotFound(new { success = false, message = "Không tìm thấy công thức hoặc cập nhật thất bại!" });


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
        [Authorize("ROLE_STAFF")]
        [Authorize("ROLE_ADMIN")]
        [Authorize("ROLE_MANAGER")]
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
            try
            {
                var recipes = await _recipeService.GetAllRecipesAsync(
             search, sortBy, isDescending, recipeStatus, categoryId, recipeLevel, startDate, endDate, page, pageSize, userId);

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

        [HttpPut("status/{id}")]
        [Authorize("ROLE_STAFF")]
        [Authorize("ROLE_ADMIN")]
        [Authorize("ROLE_MANAGER")]
        public async Task<IActionResult> UpdateRecipeStatus(Guid id, RecipeStatusEnum requestStatus)
        {
            try
            {
                var isUpdated = await _recipeService.UpdateRecipeStatusAsync(id, requestStatus);
                if (isUpdated == null)
                    return NotFound(new { success = false, message = "Không tìm thấy công thức hoặc cập nhật thất bại!" });


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
    }

}
