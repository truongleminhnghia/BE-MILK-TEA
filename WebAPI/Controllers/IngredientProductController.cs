using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Business_Logic_Layer.Services.IngredientProductService;
using Business_Logic_Layer.Services.IngredientService;
using Data_Access_Layer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

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

            var ingredientExists = await _ingredientProductService.IngredientExistsAsync(ingredientReq.IngredientId);
            if (!ingredientExists)
            {
                return BadRequest(new ApiResponse(
                    HttpStatusCode.BadRequest.GetHashCode(),
                    false,
                    "Nguyên liệu không tồn tại"));
            }


            var ingredientProduct = _mapper.Map<IngredientProduct>(ingredientReq);
            await _ingredientProductService.CreateAsync(ingredientProduct);

            return Ok(new ApiResponse(
                        HttpStatusCode.OK.GetHashCode(),
                        true,
                        "Thêm nguyên liệu vào sản phẩm thành công"));
        }
    }
}

