using System.Net;
using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Business_Logic_Layer.Services.PromotionDetailService;
using Data_Access_Layer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/promotiondetails")]
    public class PromotionDetailController : ControllerBase
    {
        private readonly IPromotionDetailService _promotionDetailService;
        private readonly IMapper _mapper;

        public PromotionDetailController(IPromotionDetailService promotionDetailService, IMapper mapper)
        {
            _promotionDetailService = promotionDetailService;
            _mapper = mapper;
        }
        // GET: Lấy danh sách hoặc chi tiết promotion detail theo ID
        [HttpGet]
        [Authorize(Roles = "ROLE_ADMIN, ROLE_STAFF, ROLE_MANAGER")]
        public async Task<IActionResult> GetPromotionDetails(
            [FromQuery] Guid? promotionId,
            [FromQuery] Guid? promotionDetailId)
        {
            if (promotionId.HasValue && promotionDetailId.HasValue)
            {
                return BadRequest(new ApiResponse(
                    (int)HttpStatusCode.BadRequest,
                    false,
                    "Chỉ được nhập một trong hai: promotionId hoặc promotionDetailId."
                ));
            }

            if (!promotionId.HasValue && !promotionDetailId.HasValue)
            {
                return BadRequest(new ApiResponse(
                    (int)HttpStatusCode.BadRequest,
                    false,
                    "Vui lòng nhập một trong hai: promotionId hoặc promotionDetailId."
                ));
            }

            if (promotionDetailId.HasValue)
            {
                var promotionDetail = await _promotionDetailService.GetbyId(promotionDetailId.Value);
                return Ok(new ApiResponse(
                    (int)HttpStatusCode.OK,
                    true,
                    promotionDetail != null ? "Lấy dữ liệu thành công!" : "Không có PromotionDetail nào có ID đó.",
                    promotionDetail
                ));
            }

            if (promotionId.HasValue)
            {
                var promotionDetails = await _promotionDetailService.GetAllPromotionDetailAsync(promotionId.Value);
                return Ok(new ApiResponse(
                    (int)HttpStatusCode.OK,
                    true,
                    promotionDetails.Count != 0 ? "Lấy dữ liệu thành công!" : "Không có PromotionDetail nào cả.",
                    promotionDetails
                ));
            }

            return BadRequest(new ApiResponse(
                (int)HttpStatusCode.BadRequest,
                false,
                "Yêu cầu không hợp lệ."
            ));
        }
        // POST: Tạo mới PromotionDetail
        [HttpPost("{promotionId}")]
        [Authorize(Roles = "ROLE_STAFF")]
        public async Task<IActionResult> AddPromotionDetail(Guid promotionId, [FromBody] PromotionDetailRequest promotionDetailRequest)
        {
            if (promotionDetailRequest == null)
            {
                return BadRequest(new { message = "Invalid promotion detail data" });
            }

            // Tạo entity từ request
            var promotionDetailEntity = _mapper.Map<PromotionDetail>(promotionDetailRequest);

            // ✅ Gán PromotionId lấy từ URL
            promotionDetailEntity.PromotionId = promotionId;

            // Lưu vào DB
            var createdPromotionDetail = await _promotionDetailService.CreateAsync(promotionDetailEntity);

            return Ok(new ApiResponse(
                (int)HttpStatusCode.Created,
                true,
                "Tạo PromotionDetail thành công!",
                createdPromotionDetail
            ));
        }
        // PUT: Cập nhật PromotionDetail theo ID
        [HttpPut("{id}")]
        [Authorize(Roles = "ROLE_STAFF")]
        public async Task<IActionResult> UpdatePromotionDetail(
            Guid id,
            [FromBody] PromotionDetailUpdateRequest promotionDetailRequest)
        {
            if (promotionDetailRequest == null)
            {
                return BadRequest(new ApiResponse(
                    (int)HttpStatusCode.BadRequest,
                    false,
                    "Data không hợp lệ"
                ));
            }

            var promotionDetailEntity = _mapper.Map<PromotionDetail>(promotionDetailRequest);
            var updatedPromotionDetail = await _promotionDetailService.UpdateAsync(id, promotionDetailEntity);

            if (updatedPromotionDetail == null)
            {
                return NotFound(new ApiResponse(
                    (int)HttpStatusCode.NotFound,
                    false,
                    "Không tìm thấy PromotionDetail"
                ));
            }

            return Ok(new ApiResponse(
                (int)HttpStatusCode.OK,
                true,
                "Cập nhật PromotionDetail thành công!",
                updatedPromotionDetail
            ));
        }

    }
}
