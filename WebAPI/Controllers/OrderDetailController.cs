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
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailController : ControllerBase
    {
        private readonly IOrderDetailService _orderDetailService;
        private readonly IMapper _mapper;

        public OrderDetailController(IOrderDetailService orderDetailService, IMapper mapper)
        {
            _orderDetailService = orderDetailService;
            _mapper = mapper;
        }
        //Get all
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            List<OrderDetail> orderDetails = await _orderDetailService.GetAllOrderDetailsAsync();
            if (orderDetails.Count == 0)
            {
                return Ok(new ApiResponse(
                    ((int)HttpStatusCode.OK),
                    true,
                    "Không có đơn hàng nào cả.",
                    null
                ));
            }
            return Ok(new ApiResponse(
                    ((int)HttpStatusCode.OK),
                    true,
                    "Lấy dữ liệu thành công!",
                    orderDetails
                ));
        }

        //Get by id
        [HttpGet("{orderDetailId}")]
        public async Task<IActionResult> GetById(Guid orderDetailId)
        {
            var orderDetails = await _orderDetailService.GetByIdAsync(orderDetailId);
            if (orderDetails == null)
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
                    orderDetails
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
