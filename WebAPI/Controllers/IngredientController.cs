
using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Business_Logic_Layer.Services.IngredientService;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebAPI.Controllers
{
    [Route("api/v1/ingredients")]
    [ApiController]
    public class IngredientController : ControllerBase
    {
        private readonly IIngredientService _ingredientService;
        private readonly IMapper _mapper;

        public IngredientController(IIngredientService ingredientService, IMapper mapper)
        {
            _ingredientService = ingredientService;
            _mapper = mapper;
        }

        //Lấy danh sách tất cả nguyên liệu
        // pagesize, currentPage, total, conditionm,
        // Get All, GET (bybId, email, code)
        [HttpGet]
        public async Task<IActionResult> GetAll(
                        [FromQuery] string? search,
                        [FromQuery] Guid? categoryId,
                        [FromQuery] IngredientStatus? status,
                        [FromQuery] string? sortBy,
                        [FromQuery] bool isDescending = false,
                        [FromQuery] int page = 1,
                        [FromQuery] int pageSize = 10,
                        [FromQuery] DateTime? startDate = null,
                        [FromQuery] DateTime? endDate = null
                        )

        {
            var ingredients = await _ingredientService.GetAllIngredientsAsync(search, categoryId, sortBy, isDescending, page, pageSize, startDate, endDate, status);
            var ingredientResponses = _mapper.Map<List<IngredientResponse>>(ingredients);
            return Ok(new ApiResponse(
                HttpStatusCode.OK.GetHashCode(),
                true,
                "Thành công",
                ingredientResponses));
        }

        //Lấy một nguyên liệu theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var ingredient = await _ingredientService.GetIngredientByIdAsync(id);
            var ingreReponse = _mapper.Map<CategoryResponse>(ingredient);

            if (ingredient == null)
            {
                return NotFound(new ApiResponse(
                    HttpStatusCode.NotFound.GetHashCode(),
                    false,
                    "Không tìm thấy"));
            }
            return Ok(new ApiResponse(
                HttpStatusCode.OK.GetHashCode(),
                true,
                "Thành công",
                ingreReponse));
        }

        //Thêm mới nguyên liệu
        [HttpPost]
        [Authorize(Roles = "ROLE_STAFF")]
        public async Task<IActionResult> Add(
            [FromBody] IngredientRequest ingredientRequest
        )
        {
            if (ingredientRequest == null)
            {
                return BadRequest(new ApiResponse(
                    HttpStatusCode.BadRequest.GetHashCode(),
                    false,
                    "Data không hợp lệ"));
            }

            var ingredient = _mapper.Map<Ingredient>(ingredientRequest);
            await _ingredientService.CreateIngredientAsync(ingredient);


            return Ok(new ApiResponse(
                HttpStatusCode.OK.GetHashCode(),
                true,
                "Tạo thành công"));
        }

        //Cập nhật nguyên liệu
        [HttpPut("{id}")]
        [Authorize(Roles = "ROLE_STAFF")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] IngredientRequest ingredient
        )
        {
            if (ingredient == null || id == Guid.Empty)
            {
                return BadRequest(new ApiResponse(
                    HttpStatusCode.BadRequest.GetHashCode(),
                    false,
                    "Data không hợp lệ"));
            }

            var existingIngredient = await _ingredientService.GetIngredientByIdAsync(id);
            if (existingIngredient == null)
            {
                return NotFound(new ApiResponse(
                    HttpStatusCode.NotFound.GetHashCode(),
                    false,
                    "Không tìm thấy nguyên liệu"));
            }

            var ingredientEntity = _mapper.Map<Ingredient>(ingredient);
            ingredientEntity.Id = id;

            var updatedIngredient = await _ingredientService.UpdateIngredientAsync(
                id,
                ingredientEntity
            );
            var updatedIngredientModel = _mapper.Map<Ingredient>(
                updatedIngredient
            );

            return Ok(new ApiResponse(
                HttpStatusCode.OK.GetHashCode(),
                true,
                "Cập nhật thành công"));
        }

        //  Xóa nguyên liệu
        // id nhận về là string
        [HttpDelete("{id}")]
        [Authorize("ROLE_STAFF")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var ingredient = await _ingredientService.GetIngredientByIdAsync(id);
            if (ingredient == null)
            {
                return NotFound(new { message = "Ingredient not found" });
            }

            await _ingredientService.DeleteIngredientAsync(id);
            return Ok(new ApiResponse(
                HttpStatusCode.OK.GetHashCode(),
                true,
                "Xoá thành công"));
        }
    }
}
