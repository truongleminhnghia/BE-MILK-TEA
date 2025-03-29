using System.Globalization;
using System.Net;
using AutoMapper;
using Business_Logic_Layer.Models;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Business_Logic_Layer.Services;
using Business_Logic_Layer.Services.CategoryService;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;
using MailKit.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/orderdetails")]
    public class OrderDetailController : ControllerBase
    {
        private readonly IOrderDetailService _orderDetailService;
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly IRedisService _redisCacheService;
        private const string OrderDetailCacheKey = "order_detail_cache";
        private const int CacheExpirationMinutes = 10;

        public OrderDetailController(IOrderDetailService orderDetailService, IMapper mapper, IOrderService orderService, IRedisService redisCacheService)
        {
            _orderService = orderService;
            _orderDetailService = orderDetailService;
            _mapper = mapper;
            _redisCacheService = redisCacheService;
        }
        [HttpGet]
        //[Authorize(Roles = "ROLE_ADMIN, ROLE_STAFF, ROLE_MANAGER, ROLE_STAFF")]
        public async Task<IActionResult> GetOrderDetails(
            [FromQuery] Guid? orderId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool isDescending = false)
        {

            // Generate a unique cache key based on all parameters
            var cacheKey = $"{OrderDetailCacheKey}:{orderId}:{page}:{pageSize}:{search}:{sortBy}:{isDescending}";
            // Try to get data from cache first
            var cachedData = await _redisCacheService.GetAsync<List<OrderDetailResponse>>(cacheKey);
            if (cachedData != null)
            {
                return Ok(new ApiResponse(HttpStatusCode.OK.GetHashCode(), true, "Thành công (from cache)", cachedData));
            }
            // Nếu không nhập gì ➝ Báo lỗi
            if (!orderId.HasValue)
            {
                return BadRequest(new ApiResponse(
                    ((int)HttpStatusCode.BadRequest),
                    false,
                    "Vui lòng nhập orderId "
                ));
            }
            // Nếu có orderId ➝ Lấy danh sách chi tiết đơn hàng
            if (orderId.HasValue)
            {
                var orderDetails = await _orderDetailService.GetAllOrderDetailsAsync(orderId.Value, search, sortBy, isDescending, page, pageSize);
                await _redisCacheService.SetAsync(cacheKey, orderDetails, TimeSpan.FromMinutes(CacheExpirationMinutes));
                return Ok(new ApiResponse(
                    ((int)HttpStatusCode.OK),
                    true,
                    orderDetails.Count != 0 ? "Lấy dữ liệu thành công!" : "Không có đơn hàng nào cả.",
                    orderDetails
                ));
            }

            // Trường hợp không hợp lệ (không nên xảy ra)
            return BadRequest(new ApiResponse(
                ((int)HttpStatusCode.BadRequest),
                false,
                "Yêu cầu không hợp lệ."
            ));
        }

        [HttpGet("{orderDetailId}")]
        //[Authorize(Roles = "ROLE_ADMIN, ROLE_STAFF, ROLE_MANAGER, ROLE_STAFF")]
        public async Task<IActionResult> GetById(Guid orderDetailId)
        {
            var orderDetail = await _orderDetailService.GetByIdAsync(orderDetailId);
            if (orderDetail == null)
            {
                return Ok(new ApiResponse(
                    (int)HttpStatusCode.OK,
                    true,
                    "Không tìm thấy chi tiết đơn hàng.",
                    null
                ));
            }
            return Ok(new ApiResponse(
                (int)HttpStatusCode.OK,
                true,
                "Lấy dữ liệu thành công!",
                orderDetail
            ));
        }

            ////Create
            [HttpPost]
        [Authorize(Roles = "ROLE_ADMIN, ROLE_STAFF, ROLE_MANAGER, ROLE_STAFF")]
        public async Task<IActionResult> AddOrderDetail([FromBody] CreateOrderDetailRequest orderDetails)
        {
            var order = await _orderService.GetByIdAsync(orderDetails.OrderId);
         
            if (orderDetails == null)
            {
                return BadRequest(new { message = "Invalid order detail data" });
            }
            if (order == null)
            {
                return BadRequest(new { message = "Không tồn tại order với id này" });
            }
            var orderDetailEntity = _mapper.Map<OrderDetail>(orderDetails);

            var createdOrderDetail = await _orderDetailService.CreateAsync(orderDetailEntity);
            await _redisCacheService.RemoveByPrefixAsync(OrderDetailCacheKey);
            return Ok(createdOrderDetail);

        }
        //UPDATE
        [HttpPut("{id}")]
        [Authorize(Roles = "ROLE_ADMIN, ROLE_STAFF, ROLE_MANAGER, ROLE_STAFF")]
        public async Task<IActionResult> UpdateOrderDetail(
            Guid id,
            [FromBody] CreateOrderDetailRequest orderDetailRequest
        )
        {
            if (orderDetailRequest == null)
            {
                return BadRequest(
                    new ApiResponse(
                        HttpStatusCode.BadRequest.GetHashCode(),
                        false,
                        "Data không hợp lệ"
                    )
                );
            }

            var orderDetail = _mapper.Map<OrderDetail>(orderDetailRequest);
            var updatedOrderDetail = await _orderDetailService.UpdateAsync(id, orderDetail);

            if (updatedOrderDetail == null)
            {
                return NotFound(
                    new ApiResponse(HttpStatusCode.NotFound.GetHashCode(), false, "Không tìm thấy")
                );
            }
            await _redisCacheService.RemoveByPrefixAsync(OrderDetailCacheKey);

            return Ok(
                new ApiResponse(HttpStatusCode.OK.GetHashCode(), true, "Cập nhật thành công")
            );
        }
        //Delete by id
        [HttpDelete("{orderDetailId}")]
        [Authorize(Roles = "ROLE_ADMIN, ROLE_STAFF, ROLE_MANAGER, ROLE_STAFF")]

        public async Task<IActionResult> DeleteOrder(Guid orderDetailId)
        {
            var result = await _orderDetailService.DeleteByIdAsync(orderDetailId);

            if (!result)
            {
                return NotFound(new { message = "Order Detail not found" });
            }
            await _redisCacheService.RemoveByPrefixAsync(OrderDetailCacheKey);

            return Ok(new { message = "Order Detail deleted successfully" });
        }
    }
}