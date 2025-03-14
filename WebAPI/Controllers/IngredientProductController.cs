using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Business_Logic_Layer.Services.IngredientProductService;
using Business_Logic_Layer.Services.IngredientService;
using Data_Access_Layer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/ingredientproducts")]
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
        public async Task<IActionResult> AddIngredientProduct([FromBody] IngredientProductRequest ingredientReq)
        {

            if (ingredientReq == null)
            {
                return BadRequest(new ApiResponse(
                    HttpStatusCode.BadRequest.GetHashCode(),
                    false,
                    "Dữ liệu không hợp lệ"));
            }

            try
            {
                var createdIngredientProduct = await _ingredientProductService.CreateAsync(ingredientReq);

                return Ok(new ApiResponse(
                    HttpStatusCode.OK.GetHashCode(),
                    true,
                    "Thêm nguyên liệu vào sản phẩm thành công",
                    createdIngredientProduct  // Trả về dữ liệu sau khi tạo
                ));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse(
                    HttpStatusCode.NotFound.GetHashCode(),
                    false,
                    ex.Message
                ));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(
                    HttpStatusCode.InternalServerError.GetHashCode(),
                    false,
                    "Lỗi server: " + ex.Message
                ));
            }
        }

        // GET 
        [HttpGet("{ingredientProductId}")]
        public async Task<IActionResult> GetIngredientProductById(Guid ingredientProductId)
        {
            try
            {
                var ingredientProduct = await _ingredientProductService.GetIngredientProductbyId(ingredientProductId);
                if (ingredientProduct == null)
                {
                    return NotFound(new ApiResponse(
                        HttpStatusCode.NotFound.GetHashCode(),
                        false,
                        "Không tìm thấy Ingredient Product"
                    ));
                }

                return Ok(new ApiResponse(
                    HttpStatusCode.OK.GetHashCode(),
                    true,
                    "Lấy thông tin thành công",
                    ingredientProduct
                ));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(
                    HttpStatusCode.InternalServerError.GetHashCode(),
                    false,
                    "Lỗi server: " + ex.Message
                ));
            }
        }
        // UPDATE 
        [HttpPut("{ingredientProductId}")]
        // [Authorize(Roles = "ROLE_STAFF")]
        public async Task<IActionResult> UpdateIngredientProduct(
            Guid ingredientProductId,
            [FromBody] IngredientProductRequest ingredientProductRequest)
        {
            if (ingredientProductRequest == null)
            {
                return BadRequest(new ApiResponse(
                    HttpStatusCode.BadRequest.GetHashCode(),
                    false,
                    "Dữ liệu không hợp lệ"
                ));
            }

            try
            {
                var updatedIngredientProduct = await _ingredientProductService.UpdateAsync(ingredientProductId, ingredientProductRequest);

                return Ok(new ApiResponse(
                    HttpStatusCode.OK.GetHashCode(),
                    true,
                    "Cập nhật thành công",
                    updatedIngredientProduct  // Trả về dữ liệu sau khi cập nhật
                ));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse(
                    HttpStatusCode.NotFound.GetHashCode(),
                    false,
                    ex.Message
                ));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(
                    HttpStatusCode.InternalServerError.GetHashCode(),
                    false,
                    "Lỗi server: " + ex.Message
                ));
            }
        }
    }
}

