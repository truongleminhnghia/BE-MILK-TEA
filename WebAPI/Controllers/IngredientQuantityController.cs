using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Business_Logic_Layer.Services.IngredientService;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebAPI.Controllers
{
    [Route("api/v1/ingredient-quantities")]
    [ApiController]
    public class IngredientQuantityController : ControllerBase
    {
        private readonly IIngredientQuantityService _ingredientQuantityService;

        public IngredientQuantityController(IIngredientQuantityService ingredientQuantityService)
        {
            _ingredientQuantityService = ingredientQuantityService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetAll(
                [FromQuery] string? search,
                [FromQuery] Guid? ingredientId,
                [FromQuery] ProductType? productType,
                [FromQuery] int? minQuantity,
                [FromQuery] int? maxQuantity,
                [FromQuery] DateTime? startDate,
                [FromQuery] DateTime? endDate,
                [FromQuery] string? sortBy,
                [FromQuery] bool isDescending = true,
                [FromQuery] int pageCurrent = 1,
                [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _ingredientQuantityService.GetAllAsync(
                search, ingredientId, productType, minQuantity, maxQuantity, startDate, endDate,
                sortBy, isDescending, pageCurrent, pageSize);
                if (result == null || !result.Data.Any())
                {
                    return NotFound(
                            new ApiResponse(HttpStatusCode.NotFound.GetHashCode(), false, "Không tìm thấy")
                        );
                }
                return Ok(new ApiResponse(HttpStatusCode.OK.GetHashCode(), true, "Thành công", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(HttpStatusCode.BadRequest.GetHashCode(), false, ex.Message));
            }
        }



        [HttpGet("{ingredientId}")]
        public async Task<IActionResult> GetByIngredientId(Guid ingredientId)
        {
            try
            {
                var response = await _ingredientQuantityService.GetByIngredientId(ingredientId);
                if (response == null || !response.Any())
                {
                    return NotFound(
                        new ApiResponse(HttpStatusCode.NotFound.GetHashCode(), false, "Không tìm thấy dữ liệu")
                    );
                }
                return Ok(
                    new ApiResponse(HttpStatusCode.OK.GetHashCode(), true, "Thành công", response)
                );
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new ApiResponse(HttpStatusCode.InternalServerError.GetHashCode(), false, ex.Message, null)
                );
            }
        }



        [HttpPost]
        public async Task<IActionResult> Create([FromBody] IngredientQuantityRequest request)
        {
            try
            {
                var response = await _ingredientQuantityService.CreateAsync(request);
                return Ok(new ApiResponse(HttpStatusCode.OK.GetHashCode(), true, "Tạo thành công", response));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(HttpStatusCode.InternalServerError.GetHashCode(), false, ex.Message));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] IngredientQuantityRequest request)
        {
            try
            {
                var response = await _ingredientQuantityService.UpdateAsync(id, request);
                return Ok(new ApiResponse(HttpStatusCode.OK.GetHashCode(), true, "Cập nhật thành công", response));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ApiResponse(HttpStatusCode.NotFound.GetHashCode(), false, "Không tìm thấy dữ liệu"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(HttpStatusCode.InternalServerError.GetHashCode(), false, ex.Message));
            }
        }

    }

}
