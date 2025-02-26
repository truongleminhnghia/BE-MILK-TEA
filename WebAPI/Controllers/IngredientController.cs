
using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Business_Logic_Layer.Services.IngredientService;
using Data_Access_Layer.Entities;
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
        public async Task<IActionResult> GetAll()
        {
            var ingredients = await _ingredientService.GetAllIngredientsAsync();
            var ingreReponse = _mapper.Map<CategoryResponse>(ingredients);
            return Ok(new ApiResponse(
                HttpStatusCode.OK.GetHashCode(),
                true,
                "Thành công",
                ingreReponse));
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
        //[Authorize("ROLE_STAFF")]
        public async Task<IActionResult> Add(
            [FromBody] IngredientRequest ingredient
        )
        {
            if (ingredient == null)
            {
                return BadRequest(new { message = "Invalid ingredient data" });
            }

            // Generate new Id for the ingredient
            ingredient.Id = Guid.NewGuid();

            var ingredientEntity = _mapper.Map<Data_Access_Layer.Entities.Ingredient>(ingredient);
            var createdIngredient = await _ingredientService.CreateIngredientAsync(
                ingredientEntity
            );
            var createdIngredientModel = _mapper.Map<Ingredient>(
                createdIngredient
            );

            return CreatedAtAction(
                nameof(Get),
                new { id = createdIngredientModel.Id },
                createdIngredientModel
            );
        }

        //Cập nhật nguyên liệu
        [HttpPut("{id}")]
        //[Authorize("ROLE_STAFF")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] IngredientRequest ingredient
        )
        {
            if (ingredient == null || id == Guid.Empty)
            {
                return BadRequest(new { message = "Invalid ingredient data" });
            }

            var existingIngredient = await _ingredientService.GetIngredientByIdAsync(id);
            if (existingIngredient == null)
            {
                return NotFound(new { message = "Ingredient not found" });
            }

            var ingredientEntity = _mapper.Map<Data_Access_Layer.Entities.Ingredient>(ingredient);
            ingredientEntity.Id = id;

            var updatedIngredient = await _ingredientService.UpdateIngredientAsync(
                id,
                ingredientEntity
            );
            var updatedIngredientModel = _mapper.Map<Ingredient>(
                updatedIngredient
            );

            return Ok(updatedIngredientModel);
        }

        //  Xóa nguyên liệu
        // id nhận về là string
        [HttpDelete("{id}")]
        //[Authorize("ROLE_STAFF")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var ingredient = await _ingredientService.GetIngredientByIdAsync(id);
            if (ingredient == null)
            {
                return NotFound(new { message = "Ingredient not found" });
            }

            await _ingredientService.DeleteIngredientAsync(id);
            return NoContent();
        }
    }
}
