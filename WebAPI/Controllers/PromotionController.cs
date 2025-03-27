﻿using System.Net;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Business_Logic_Layer.Services.PromotionService;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/promotions")]
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
        [Authorize(Roles = "ROLE_STAFF")]
        [Authorize(Roles = "ROLE_MANAGER")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> GetPromotion(
            [FromQuery] Guid userId,
    [FromQuery] bool? isActive = null,
    [FromQuery] string? promotionCode = null,
    [FromQuery] string? promotionName = null,
    [FromQuery] PromotionType? promotionType = null,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string? search = null,
    [FromQuery] string? sortBy = null,
    [FromQuery] bool isDescending = false,
    [FromQuery] DateOnly? startDate = null,
    [FromQuery] DateOnly? endDate = null
    )
        {
            try
            {
                if (startDate.HasValue && endDate.HasValue && startDate > endDate)
                {
                    return BadRequest(new ApiResponse(
                        (int)HttpStatusCode.BadRequest,
                        false,
                        "StartDate không được lớn hơn EndDate."
                    ));
                }

                var promotions = await _promotionService.GetAllPromotions(
                    search, sortBy, isDescending, promotionType, promotionCode, promotionName,
                    startDate, endDate, page, pageSize, userId);

                return Ok(new ApiResponse(
                    (int)HttpStatusCode.OK,
                    true,
                    promotions.Data.Any() ? "Lấy dữ liệu thành công!" : "Không có Promotion nào phù hợp.",
                    promotions
                ));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy danh sách Promotion: {ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse(
                    (int)HttpStatusCode.InternalServerError,
                    false,
                    "Đã xảy ra lỗi trong quá trình xử lý yêu cầu."
                ));
            }
        }


        ////Get by id
        [HttpGet("{promotionId}")]
        [Authorize(Roles = "ROLE_STAFF")]
        public async Task<IActionResult> GetById(Guid promotionId)
        {
            PromotionResponse promotions = await _promotionService.GetByIdAsync(promotionId);
            if (promotions == null)
            {
                return Ok(new ApiResponse(
                    (int)HttpStatusCode.OK,
                    true,
                    "Không có promotion nào có ID đó cả.",
                    null
                ));
            }
            return Ok(new ApiResponse(
                    (int)HttpStatusCode.OK,
                    true,
                    "Lấy dữ liệu thành công!",
                    promotions
                ));
        }
        //Create
        [HttpPost]
        [Authorize(Roles = "ROLE_STAFF")]
        public async Task<IActionResult> AddPromotion([FromBody] PromotionRequest promotion)
        {
            if (promotion == null || promotion.promotionDetail == null || promotion.promotionDetail == null)
            {
                return BadRequest(new ApiResponse(
                    (int)HttpStatusCode.BadRequest,
                    false,
                    "Dữ liệu promotion lỗi hoặc không có PromotionDetail."
                ));
            }

            var createdPromotion = await _promotionService.CreateAsync(promotion);

            return Ok(new ApiResponse(
                (int)HttpStatusCode.OK,
                true,
                "Thêm promotion thành công!",
                createdPromotion
            ));
        }
        //UPDATE
        [HttpPut("{promotionId}")]
        [Authorize(Roles = "ROLE_STAFF")]
        public async Task<IActionResult> UpdatePromotion(
        Guid promotionId,
        [FromBody] PromotionUpdateRequest promotionUpdateRequest, [FromQuery] double maxPriceThreshold, [FromQuery] double minPriceThreshold)
        {
            try
            {
                if (promotionUpdateRequest == null)
                {
                    return BadRequest(new ApiResponse(
                        (int)HttpStatusCode.BadRequest,
                        false,
                        "Dữ liệu không hợp lệ."
                    ));
                }

                var promotion = _mapper.Map<Promotion>(promotionUpdateRequest);
                var updatedPromotion = await _promotionService.UpdateAsync(promotionId, promotion, maxPriceThreshold, minPriceThreshold);

                return Ok(new ApiResponse(
                    (int)HttpStatusCode.OK,
                    true,
                    "Cập nhật thành công.",
                    updatedPromotion
                ));
            }
            catch (ArgumentException ex) // Bắt lỗi ngày không hợp lệ
            {
                return BadRequest(new ApiResponse(
                    (int)HttpStatusCode.BadRequest,
                    false,
                    ex.Message
                ));
            }
        }

        //DELETE
        [HttpDelete("{promotionId}")]
        [Authorize(Roles = "ROLE_ADMIN,ROLE_STAFF,ROLE_MANAGER")]
        public async Task<IActionResult> DeletePromotion(Guid promotionId)
        {
            try
            {
                var deletedPromotion = await _promotionService.DeleteAsync(promotionId);
                return Ok(new ApiResponse(
                    (int)HttpStatusCode.OK,
                    true,
                    "Xóa thành công.",
                    deletedPromotion
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(
                    (int)HttpStatusCode.BadRequest,
                    false,
                    ex.Message
                ));
            }
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActivePromotion(
            [FromQuery] PromotionType? promotionType,
            [FromQuery] double? orderTotalPrice,
            [FromQuery] DateOnly? expiredDate,
            [FromQuery] bool? isActive
            )
        {
            try
            {
                var promotions = await _promotionService.GetActivePromotions(promotionType, orderTotalPrice, expiredDate, isActive);
                return Ok(new ApiResponse(
                    (int)HttpStatusCode.OK,
                    true,
                    "Lấy dữ liệu thành công!",
                    promotions
                ));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy danh sách Promotion: {ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse(
                    (int)HttpStatusCode.InternalServerError,
                    false,
                    "Đã xảy ra lỗi trong quá trình xử lý yêu cầu."
                ));
            }
        }


    }
}
