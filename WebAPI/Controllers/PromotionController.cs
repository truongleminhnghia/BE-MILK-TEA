using System.Net;
using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Business_Logic_Layer.Services.PromotionService;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/Promotions")]
    public class PromotionController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IPromotionService _promotionService;

        public PromotionController(IMapper mapper, IPromotionService promotionService)
        {
            _mapper = mapper;
            _promotionService = promotionService;
        }

        //Get all
        [HttpGet]
        public async Task<IActionResult> GetPromotion(
            [FromQuery] bool IsActive,
            [FromQuery] PromotionType? promotionType,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool isDescending = false,
            [FromQuery] DateTime? StartDate = null,
            [FromQuery] DateTime? EndDate = null
            )
        {
            try
            {
                if (StartDate.HasValue && EndDate.HasValue && StartDate > EndDate)
                {
                    return BadRequest(new ApiResponse(
                        ((int)HttpStatusCode.BadRequest),
                        false,
                        "StartDate không được lớn hơn EndDate."
                    ));
                }

                var promotions = await _promotionService.GetAllPromotionAsync(
                 IsActive, search, sortBy, isDescending, promotionType, StartDate, EndDate, page, pageSize
             );

                return Ok(new ApiResponse(
                    ((int)HttpStatusCode.OK),
                    true,
                    promotions != null ? "Lấy dữ liệu thành công!" : " Không có Promotion nào phù hợp",
                    promotions
                ));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy danh sách Promotion: {ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse(
                    ((int)HttpStatusCode.InternalServerError),
                    false,
                    "Đã xảy ra lỗi trong quá trình xử lý yêu cầu."
                ));
            }
        }
        ////Get by id
        [HttpGet("{promotionId}")]
        public async Task<IActionResult> GetById(Guid promotionId)
        {
            PromotionResponse promotions = await _promotionService.GetByIdAsync(promotionId);
            if (promotions == null)
            {
                return Ok(new ApiResponse(
                    ((int)HttpStatusCode.OK),
                    true,
                    "Không có promotion nào có ID đó cả.",
                    null
                ));
            }
            return Ok(new ApiResponse(
                    ((int)HttpStatusCode.OK),
                    true,
                    "Lấy dữ liệu thành công!",
                    promotions
                ));
        }
        //Create
        [HttpPost]
        public async Task<IActionResult> AddPromotion([FromBody] PromotionRequest promotion)
        {
            try
            {
                if (promotion == null)
                {
                    return BadRequest(new ApiResponse(
                        HttpStatusCode.BadRequest.GetHashCode(),
                        false,
                        "Dữ liệu promotion lỗi."
                    ));
                }

                var createdPromotion = await _promotionService.CreateAsync(promotion);

                return Ok(new ApiResponse(
                    HttpStatusCode.OK.GetHashCode(),
                    true,
                    "Thêm promotion thành công!",
                    createdPromotion
                ));
            }
            catch (ArgumentException ex) 
            {
                return BadRequest(new ApiResponse(
                    HttpStatusCode.BadRequest.GetHashCode(),
                    false,
                    ex.Message
                ));
            }
        }
        //UPDATE
        [HttpPut("{promotionId}")]
        public async Task<IActionResult> UpdatePromotion(
    Guid promotionId,
    [FromBody] PromotionUpdateRequest promotionUpdateRequest
)
        {
            try
            {
                if (promotionUpdateRequest == null)
                {
                    return BadRequest(new ApiResponse(
                        HttpStatusCode.BadRequest.GetHashCode(),
                        false,
                        "Dữ liệu không hợp lệ."
                    ));
                }

                var promotion = _mapper.Map<Promotion>(promotionUpdateRequest);
                var updatedPromotion = await _promotionService.UpdateAsync(promotionId, promotion);

                return Ok(new ApiResponse(
                    HttpStatusCode.OK.GetHashCode(),
                    true,
                    "Cập nhật thành công.",
                    updatedPromotion
                ));
            }
            catch (ArgumentException ex) // Bắt lỗi ngày không hợp lệ
            {
                return BadRequest(new ApiResponse(
                    HttpStatusCode.BadRequest.GetHashCode(),
                    false,
                    ex.Message
                ));
            }
        }
    }
}
