using System.Net;
using AutoMapper;
using Business_Logic_Layer.Models;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Business_Logic_Layer.Services;
using Business_Logic_Layer.Services.CategoryService;
using Data_Access_Layer.Entities;
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
        private readonly IMapper _mapper;

        public OrderDetailController(IOrderDetailService orderDetailService, IMapper mapper)
        {
            _orderDetailService = orderDetailService;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetOrderDetails(
            [FromQuery] Guid? orderId,
            [FromQuery] Guid? orderDetailId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool isDescending = false)
        {
            // Nếu nhập cả 2 ➝ Báo lỗi
            if (orderId.HasValue && orderDetailId.HasValue)
            {
                return BadRequest(new ApiResponse(
                    ((int)HttpStatusCode.BadRequest),
                    false,
                    "Chỉ được nhập một trong hai: orderId hoặc orderDetailId."
                ));
            }

            // Nếu không nhập gì ➝ Báo lỗi
            if (!orderId.HasValue && !orderDetailId.HasValue)
            {
                return BadRequest(new ApiResponse(
                    ((int)HttpStatusCode.BadRequest),
                    false,
                    "Vui lòng nhập một trong hai: orderId hoặc orderDetailId."
                ));
            }

            // Nếu có orderDetailId ➝ Lấy chi tiết đơn hàng
            if (orderDetailId.HasValue)
            {
                var orderDetail = await _orderDetailService.GetByIdAsync(orderDetailId.Value);
                return Ok(new ApiResponse(
                    ((int)HttpStatusCode.OK),
                    true,
                    orderDetail != null ? "Lấy dữ liệu thành công!" : "Không có đơn hàng nào có ID đó cả.",
                    orderDetail
                ));
            }

            // Nếu có orderId ➝ Lấy danh sách chi tiết đơn hàng
            if (orderId.HasValue)
            {
                var orderDetails = await _orderDetailService.GetAllOrderDetailsAsync(orderId.Value, search, sortBy, isDescending, page, pageSize);
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
        ////Create
        [HttpPost]
        public async Task<IActionResult> AddOrderDetail([FromBody] OrderDetailRequest orderDetails)
        {
            if (orderDetails == null)
            {
                return BadRequest(new { message = "Invalid order detail data" });
            }
            var orderDetailEntity = _mapper.Map<OrderDetail>(orderDetails);

            var createdOrderDetail = await _orderDetailService.CreateAsync(orderDetailEntity);
            return Ok(createdOrderDetail);
        }
        //UPDATE
        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateOrderDetail(
            Guid id,
            [FromBody] OrderDetailRequest orderDetailRequest
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

            return Ok(
                new ApiResponse(HttpStatusCode.OK.GetHashCode(), true, "Cập nhật thành công")
            );
        }
        //Delete by id
        [HttpDelete("{orderDetailId}")]
        public async Task<IActionResult> DeleteOrder(Guid orderDetailId)
        {
            var result = await _orderDetailService.DeleteByIdAsync(orderDetailId);

            if (!result)
            {
                return NotFound(new { message = "Order Detail not found" });
            }

            return Ok(new { message = "Order Detail deleted successfully" });
        }
    }
}
