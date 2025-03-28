using System.Diagnostics;
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
        [Authorize(Roles = "ROLE_STAFF,ROLE_MANAGER,ROLE_ADMIN")]
        public async Task<IActionResult> SearchIngredients(
                        [FromQuery] string? search,
                        [FromQuery] string? categorySearch,
                        [FromQuery] Guid? categoryId,
                        [FromQuery] string? sortBy,
                        [FromQuery] DateOnly? startDate,
                        [FromQuery] DateOnly? endDate,
                        [FromQuery] IngredientStatus? status,
                        [FromQuery] IngredientType? ingredientType,
                        [FromQuery] decimal? minPrice,
                        [FromQuery] decimal? maxPrice,
                        [FromQuery] bool? isSale,
                        [FromQuery] bool isDescending = false,
                        [FromQuery] int pageCurrent = 1,
                        [FromQuery] int pageSize = 10
                        )
        {
            var result = await _ingredientService.GetAllAsync(
                search, categorySearch, categoryId, sortBy, isDescending,
                pageCurrent, pageSize, startDate, endDate, status, minPrice, maxPrice, isSale, ingredientType
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

        [HttpGet]
        [Authorize(Roles = "ROLE_STAFF,ROLE_MANAGER,ROLE_ADMIN")]
        public async Task<IActionResult> GetById( [FromQuery] Guid? id, [FromQuery] string? code)
        {
            try
            {
                var ingreReponse = await _ingredientService.GetByIdOrCode(id, code);
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
        [Authorize(Roles = "ROLE_STAFF")]
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
        [Authorize(Roles = "ROLE_STAFF")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateIngredientRequest request)
        {
            if (id != null && request != null)
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

        [HttpDelete("{id}")]
        [Authorize(Roles = "ROLE_STAFF,ROLE_MANAGER,ROLE_ADMIN")]
        public async Task<IActionResult> UpdateStatus(Guid id, bool? status)
        {
            try
            {                
                if (status == null)
                {
                    return BadRequest(new ApiResponse(HttpStatusCode.BadRequest.GetHashCode(), false, "Thay đổi trạng thái thất bại", null));
                }
                bool result = await _ingredientService.ChangeStatus(id);
                if (!result)
                {
                    return BadRequest(new ApiResponse(HttpStatusCode.BadRequest.GetHashCode(), false, "Thay đổi trạng thái thất bại", null));
                }
                return Ok(new ApiResponse(HttpStatusCode.OK.GetHashCode(), true, "Thay đổi trạng thái thành công", null));
            }
            catch(Exception ex)
            {
                return BadRequest(new ApiResponse(HttpStatusCode.InternalServerError.GetHashCode(), false, ex.Message, null));
            }
        }
    }
}
