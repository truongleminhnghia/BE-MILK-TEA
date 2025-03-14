﻿using System.Diagnostics;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using AutoMapper;
using Business_Logic_Layer.Models;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Business_Logic_Layer.Services;
using Business_Logic_Layer.Services.IngredientService;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/v1/ingredients")]
    [ApiController]
    public class IngredientController : ControllerBase
    {
        private readonly IIngredientService _ingredientService;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;
        private readonly ICategoryService _categoryService;

        public IngredientController(IImageService imageService, IIngredientService ingredientService, IMapper mapper, ICategoryService categoryService
        )
        {
            _imageService = imageService;
            _ingredientService = ingredientService;
            _mapper = mapper;
            _categoryService = categoryService;
        }

        // [HttpGet]
        // public async Task<IActionResult> GetAll(
        //     [FromQuery] string? search,
        //     [FromQuery] Guid? categoryId,
        //     [FromQuery] IngredientStatus? status,
        //     [FromQuery] string? sortBy,
        //     [FromQuery] bool isDescending = false,
        //     [FromQuery] int page = 1,
        //     [FromQuery] int pageSize = 10,
        //     [FromQuery] DateTime? startDate = null,
        //     [FromQuery] DateTime? endDate = null
        // )
        // {
        //     var ingredients = await _ingredientService.GetAllIngredientsAsync(
        //         search,
        //         categoryId,
        //         sortBy,
        //         isDescending,
        //         page,
        //         pageSize,
        //         startDate,
        //         endDate,
        //         status
        //     );
        //     var ingredientResponses = _mapper.Map<List<IngredientResponse>>(ingredients);
        //     return Ok(
        //         new ApiResponse(
        //             HttpStatusCode.OK.GetHashCode(),
        //             true,
        //             "Thành công",
        //             ingredientResponses
        //         )
        //     );
        // }
        
        [HttpGet]
        public async Task<IActionResult> SearchIngredients(
                        [FromQuery] string? search,
                        [FromQuery] string? categorySearch,
                        [FromQuery] Guid? categoryId,
                        [FromQuery] string? sortBy,
                        [FromQuery] DateTime? startDate,
                        [FromQuery] DateTime? endDate,
                        [FromQuery] IngredientStatus? status,
                        [FromQuery] decimal? minPrice,
                        [FromQuery] decimal? maxPrice,
                        [FromQuery] bool? isSale,
                        [FromQuery] bool isDescending = false,
                        [FromQuery] int pageCurrent = 1,
                        [FromQuery] int pageSize = 10)
        {
            var result = await _ingredientService.GetAllAsync(
                search, categorySearch, categoryId, sortBy, isDescending,
                pageCurrent, pageSize, startDate, endDate, status, minPrice, maxPrice, isSale
            );
            if (result == null || !result.Data.Any())
            {
                return NotFound(
                        new ApiResponse(HttpStatusCode.NotFound.GetHashCode(), false, "Không tìm thấy")
                    );
            }
            return Ok(
                    new ApiResponse(HttpStatusCode.OK.GetHashCode(), true, "Thành công", result)
                );
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var ingreReponse = await _ingredientService.GetById(id);
                if (ingreReponse == null)
                {
                    return NotFound(
                        new ApiResponse(HttpStatusCode.NotFound.GetHashCode(), false, "Không tìm thấy")
                    );
                }
                return Ok(
                    new ApiResponse(HttpStatusCode.OK.GetHashCode(), true, "Thành công", ingreReponse)
                );
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(HttpStatusCode.InternalServerError.GetHashCode(), false, ex.Message, null));
            }
        }


        //[Authorize(Roles = "ROLE_STAFF")]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] IngredientRequest ingredientRequest)
        {
            try
            {
                if (ingredientRequest == null)
                {
                    return BadRequest(
                        new ApiResponse(
                            HttpStatusCode.BadRequest.GetHashCode(),
                            false,
                            "Thông tin nguyên liệu không được bỏ trống"
                        )
                    );
                }
                var ingredient = await _ingredientService.CreateIngredientAsync(ingredientRequest);
                if (ingredient == null)
                {
                    return BadRequest(
                        new ApiResponse(
                            HttpStatusCode.BadRequest.GetHashCode(),
                            false,
                            "Tạo không thành công",
                            null
                        )
                    );
                }
                return Ok(new ApiResponse(HttpStatusCode.OK.GetHashCode(), true, "Tạo thành công", ingredient));
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new ApiResponse(
                        HttpStatusCode.InternalServerError.GetHashCode(),
                        false,
                        ex.Message,
                        null
                    )
                );
            }
        }

        [HttpPut("{id}")]
        //[Authorize(Roles = "ROLE_STAFF")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateIngredientRequest request, bool? status = false)
        {
            if (status == true && id != null)
            {
                bool result = await _ingredientService.ChangeStatus(id);
                if (!result)
                {
                    return BadRequest(new ApiResponse(HttpStatusCode.BadRequest.GetHashCode(), false, "Thay đổi trạng thái thất bại", null));
                }
                return Ok(new ApiResponse(HttpStatusCode.OK.GetHashCode(), true, "Thay đổi trạng thái thành công", null));
            }
            else if (id != null && request != null)
            {
                IngredientResponse ingredientResponse = await _ingredientService.Update(id, request);
                if (ingredientResponse == null)
                {
                    return BadRequest(new ApiResponse(HttpStatusCode.BadRequest.GetHashCode(), false, "Cập nhật nguyên liệu thất bại", null));
                }
                return Ok(new ApiResponse(HttpStatusCode.OK.GetHashCode(), true, "Cập nhật nguyên liệu thành công", ingredientResponse));
            }
            else
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                                    new ApiResponse(HttpStatusCode.BadRequest.GetHashCode(), false, "Thay đổi trạng thái thất bại", null));
            }

        }
    }
}
