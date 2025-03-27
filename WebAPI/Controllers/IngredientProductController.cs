using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Business_Logic_Layer.Services.IngredientProductService;
using Business_Logic_Layer.Services.IngredientService;
using Data_Access_Layer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/ingredient-products")]
    public class IngredientProductController : ControllerBase
    {
        private readonly IIngredientProductService _ingredientProductService;
        private readonly IMapper _mapper;
        public IngredientProductController(IIngredientProductService ingredientProductService, IMapper mapper)
        {
            _ingredientProductService = ingredientProductService;
            _mapper = mapper;
        }
        //CREATE
        [HttpPost]
        [Authorize("ROLE_STAFF")]
        public async Task<IActionResult> AddIngredientProduct([FromBody] IngredientProductRequest ingredientReq, [FromQuery] bool isCart)
        {
            try
            {
                var ingredientProduct = await _ingredientProductService.CreateAsync(ingredientReq, isCart);
                return Ok(new ApiResponse(
                    HttpStatusCode.OK.GetHashCode(),
                    true,
                    "Thêm nguyên liệu vào sản phẩm thành công",
                    ingredientProduct));
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponse(
                    HttpStatusCode.BadRequest.GetHashCode(),
                    false,
                    e.Message));
            }
        }

        [HttpGet]
        [Authorize("ROLE_STAFF")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            try
            {
                var ingredientProduct = await _ingredientProductService.GetIngredientProductbyId(id);
                if (ingredientProduct == null)
                {
                    return NotFound(new ApiResponse(
                        HttpStatusCode.NotFound.GetHashCode(),
                        false,
                        "Không tìm thấy sản phẩm"));
                }
                var ingredientProductResponse = _mapper.Map<IngredientProductResponse>(ingredientProduct);
                return Ok(new ApiResponse(
                    HttpStatusCode.OK.GetHashCode(),
                    true,
                    "Thành công",
                    ingredientProductResponse));
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponse(
                    HttpStatusCode.BadRequest.GetHashCode(),
                    false,
                    e.Message));
            }
        }

        [HttpPut("{id}")]
        [Authorize("ROLE_STAFF")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] IngredientProductRequest request, [FromQuery] bool isCart)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new ApiResponse(
                        HttpStatusCode.BadRequest.GetHashCode(),
                        false,
                        "Dữ liệu không hợp lệ"));
                }
                var ingredientProductUpdate = _mapper.Map<IngredientProduct>(request);
                await _ingredientProductService.UpdateAsync(id, request, isCart);
                return Ok(new ApiResponse(
                    HttpStatusCode.OK.GetHashCode(),
                    true,
                    "Cập nhật sản phẩm thành công"));
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponse(
                    HttpStatusCode.BadRequest.GetHashCode(),
                    false,
                    e.Message));
            }
        }

        [HttpDelete("{id}")]
        [Authorize("ROLE_STAFF")]
        [Authorize("ROLE_ADMIN")]
        [Authorize("ROLE_MANAGER")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            try
            {
                var result = await _ingredientProductService.DeleteAsync(id);
                if (!result)
                {
                    return NotFound(new ApiResponse(
                        HttpStatusCode.NotFound.GetHashCode(),
                        false,
                        "Không tìm thấy sản phẩm"));
                }
                return Ok(new ApiResponse(
                    HttpStatusCode.OK.GetHashCode(),
                    true,
                    "Xóa sản phẩm thành công"));
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponse(
                    HttpStatusCode.BadRequest.GetHashCode(),
                    false,
                    e.Message));
            }
        }

        [HttpGet("by-ingredient/{ingredientId}")]
        [Authorize("ROLE_STAFF")]
        public async Task<IActionResult> GetByIngredientId(Guid? ingredientId, string? ingredientCode)
        {
            try
            {
                var ingredientProducts = await _ingredientProductService.GetByIngredientIdOrCodeAsync(ingredientId, ingredientCode);
                if (ingredientProducts == null || !ingredientProducts.Any())
                {
                    return NotFound("Không tìm thấy Ingredient Product.");
                }
                return Ok(new ApiResponse(
                        HttpStatusCode.OK.GetHashCode(),
                        true,
                        "Lấy danh sách sản phẩm thành công"));
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponse(
                    HttpStatusCode.BadRequest.GetHashCode(),
                    false,
                    "Đã có lỗi khi lấy danh sách: " + e.Message));
            }
        }
    }
}

