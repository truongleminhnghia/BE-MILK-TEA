using System.Collections.Generic;
using System.Linq;
using System.Net;
using AutoMapper;
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
    [Route("api/v1/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public OrderController(IOrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        [HttpGet("search")]
        //[Authorize(Roles = "ROLE_ADMIN, ROLE_STAFF, ROLE_MANAGER")]
        public async Task<IActionResult> GetOrder(
            [FromQuery] Guid accountId,
            [FromQuery] OrderStatus? orderStatus,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool isDescending = false,
            [FromQuery] DateTime? orderDate = null)
        {
            var orders = await _orderService.GetAllAsync(accountId, search, sortBy, isDescending, orderStatus, orderDate, page, pageSize);

            return Ok(new ApiResponse(
                (int)HttpStatusCode.OK,
                true,
                orders.Count != 0 ? "Lấy dữ liệu thành công!" : "Không có đơn hàng nào cả.",
                orders
            ));
        }

        // Get order by ID or Order Code
        [HttpGet]
        //[Authorize(Roles = "ROLE_ADMIN, ROLE_STAFF, ROLE_MANAGER, ROLE_STAFF")]
        public async Task<IActionResult> GetById(
            [FromQuery] Guid? orderId,
            [FromQuery] string? orderCode)
        {
            // Kiểm tra chỉ được nhập một trong hai giá trị
            if ((orderId.HasValue && !string.IsNullOrEmpty(orderCode)) || (!orderId.HasValue && string.IsNullOrEmpty(orderCode)))
            {
                return BadRequest(new ApiResponse(
                    (int)HttpStatusCode.BadRequest,
                    false,
                    "Vui lòng chỉ nhập một trong hai giá trị: orderId hoặc orderCode.",
                    null
                ));
            }

            OrderResponse? orders = null;

            if (orderId.HasValue)
            {
                orders = await _orderService.GetByIdAsync(orderId.Value);
            }
            else if (!string.IsNullOrEmpty(orderCode))
            {
                orders = await _orderService.GetByCodeAsync(orderCode);
            }

            if (orders == null)
            {
                return Ok(new ApiResponse(
                    (int)HttpStatusCode.OK,
                    true,
                    "Không tìm thấy đơn hàng.",
                    null
                ));
            }

            return Ok(new ApiResponse(
                (int)HttpStatusCode.OK,
                true,
                "Lấy dữ liệu thành công!",
                orders
            ));
        }

        //Create
        [HttpPost]
        [Authorize(Roles = "ROLE_ADMIN, ROLE_STAFF, ROLE_MANAGER, ROLE_STAFF")]
        public async Task<IActionResult> AddOrder([FromBody] OrderRequest order)
        {
            if (order == null || order.orderDetailList == null || !order.orderDetailList.Any())
            {
                return BadRequest(new ApiResponse(
                    HttpStatusCode.BadRequest.GetHashCode(),
                    false,
                    "Dữ liệu order lỗi hoặc không có OrderDetail"
                    ));
            }

            var createdOrder = await _orderService.CreateAsync(order);
                return Ok(new ApiResponse(
                    HttpStatusCode.OK.GetHashCode(),
                    true,
                    "Thêm đơn hàng thành công",
                    createdOrder
                    ));
        }
        //UPDATE
        [HttpPut("{orderId}")]
        [Authorize(Roles = "ROLE_ADMIN, ROLE_STAFF, ROLE_MANAGER, ROLE_STAFF")]
        //[Authorize(Roles = "ROLE_STAFF")]
        public async Task<IActionResult> UpdateOrder(
            Guid orderId,
            [FromBody] OrderUpdateRequest orderUpdateRequest
        )
        {
            if (orderUpdateRequest == null)
            {
                return BadRequest(
                    new ApiResponse(
                        HttpStatusCode.BadRequest.GetHashCode(),
                        false,
                        "Data không hợp lệ"
                    )
                );
            }

            var order = _mapper.Map<Order>(orderUpdateRequest);
            var updatedOrder = await _orderService.UpdateStatus(orderId, order);

            if (updatedOrder == null)
            {
                return NotFound(
                    new ApiResponse(HttpStatusCode.NotFound.GetHashCode(), false, "Không tìm thấy")
                );
            }

            return Ok(
                new ApiResponse(HttpStatusCode.OK.GetHashCode(), true, "Cập nhật thành công")
            );
        }


    }
}