using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Business_Logic_Layer.Models;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Services;
using Business_Logic_Layer.Services.CategoryService;
using Business_Logic_Layer.Services.IngredientService;
using Data_Access_Layer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/v1/ingredients")]
    [ApiController]
    public class IngredientController : ControllerBase
    {
        private readonly IIngredientService _ingredientService;
        private readonly IMapper _mapper;
        private readonly ICategoryService _categorytService;

        public IngredientController(
            ICategoryService categoryService,
            IIngredientService ingredientService,
            IMapper mapper
        )
        {
            _ingredientService = ingredientService;
            _categorytService = categoryService;
            _mapper = mapper;
        }

        //Lấy danh sách tất cả nguyên liệu
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var ingredients = await _ingredientService.GetAllIngredientsAsync();
            return Ok(ingredients);
        }

        //Lấy một nguyên liệu theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var ingredient = await _ingredientService.GetIngredientByIdAsync(id);
            if (ingredient == null)
            {
                return NotFound(new { message = "Ingredient not found" });
            }
            return Ok(ingredient);
        }

        

        //Thêm mới nguyên liệu
        [HttpPost]
        public async Task<IActionResult> Add(
            [FromBody] Business_Logic_Layer.Models.Ingredient ingredient
        )
        {
            if (ingredient == null)
            {
                return BadRequest(new { message = "Invalid ingredient data" });
            }
            var ingredientEntity = _mapper.Map<Data_Access_Layer.Entities.Ingredient>(ingredient);
            var createdIngredient = await _ingredientService.CreateIngredientAsync(
                ingredientEntity
            );
            var createdIngredientModel = _mapper.Map<Business_Logic_Layer.Models.Ingredient>(
                createdIngredient
            );
            return CreatedAtAction(
                nameof(Get),
                new { id = createdIngredientModel.Id },
                createdIngredientModel
            );
        }

        // 🟢 Cập nhật nguyên liệu
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] Business_Logic_Layer.Models.Ingredient ingredient
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
            var updatedIngredientModel = _mapper.Map<Business_Logic_Layer.Models.Ingredient>(
                updatedIngredient
            );

            return Ok(updatedIngredientModel);
        }

        //  Xóa nguyên liệu
        [HttpDelete("{id}")]
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
