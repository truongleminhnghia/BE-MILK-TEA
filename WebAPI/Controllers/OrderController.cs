﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Business_Logic_Layer.Services;
using Business_Logic_Layer.Services.CategoryService;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;
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

        //Get all
        [HttpGet("{accountId}")]

        public async Task<IActionResult> GetAll(
            Guid accountId,
            [FromQuery] OrderStatus? orderStatus,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool isDescending = false,
            [FromQuery] DateTime? orderDate = null)
        {
            var orders = await _orderService.GetAllAsync(accountId, search, sortBy, isDescending, orderStatus, orderDate, page, pageSize);
            if (orders.Count != 0)
            {
                return Ok(new ApiResponse(
                    ((int)HttpStatusCode.OK),
                    true,
                    "Lấy dữ liệu thành công!",
                    orders
                ));
            }
            return Ok(new ApiResponse(
            ((int)HttpStatusCode.OK),
            true,
            "Không có đơn hàng nào cả.",
            null
        ));
        }
        //Get by id
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetById(Guid orderId)
        {
            OrderResponse orders = await _orderService.GetByIdAsync(orderId);
            if (orders == null)
            {
                return Ok(new ApiResponse(
                    ((int)HttpStatusCode.OK),
                    true,
                    "Không có đơn hàng nào có ID đó cả.",
                    null
                ));
            }
            return Ok(new ApiResponse(
                    ((int)HttpStatusCode.OK),
                    true,
                    "Lấy dữ liệu thành công!",
                    orders
                ));
        }
        //Create
        [HttpPost]
        public async Task<IActionResult> AddOrder([FromBody] OrderRequest order)
        {
            if (order == null)
            {
                return BadRequest(new { message = "Dữ liệu order lỗi" });
            }

            var createdOrder = await _orderService.CreateAsync(order);
            return Ok(createdOrder);
        }
        //UPDATE
        [HttpPut("{orderId}")]
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

        //Delete by id
        //[HttpDelete("{orderId}")]
        //public async Task<IActionResult> DeleteOrder(Guid orderId)
        //{
        //    var result = await _orderService.DeleteByIdAsync(orderId);

        //    if (!result)
        //    {
        //        return NotFound(new { message = "Order not found" });
        //    }

        //    return Ok(new { message = "Order deleted successfully" });
        //}
    }
}
