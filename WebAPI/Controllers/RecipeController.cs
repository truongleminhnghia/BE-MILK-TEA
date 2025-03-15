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
        public async Task<IActionResult> UpdateRecipe(Guid id, [FromBody] RecipeRequest request)
        {
            try
            {
                var updatedRecipe = await _recipeService.UpdateRecipe(id, request);
                if (updatedRecipe == null) return NotFound(new { message = "Không tìm thấy công thức hoặc cập nhật thất bại!" });

                return Ok(new ApiResponse(
                                HttpStatusCode.OK.GetHashCode(),
                                true,
                "Lấy công thức thành công",
                                updatedRecipe
                            ));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(HttpStatusCode.InternalServerError.GetHashCode(), false, ex.Message));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRecipes(
            string? search,
            string? sortBy,
            RecipeStatusEnum recipeStatusEnum,
            bool isDescending = false,
            Guid? categoryId = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int page = 1,
            int pageSize = 10)
        {
            try
            {
                var recipes = await _recipeService.GetAllRecipesAsync(
                    search, sortBy, isDescending, recipeStatusEnum, categoryId, startDate, endDate, page, pageSize);

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


    }

}
