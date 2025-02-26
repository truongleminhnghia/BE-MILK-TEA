using System.Collections.Generic;
using System.Linq;
using System.Net;
using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Business_Logic_Layer.Services;
using Data_Access_Layer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _orderService.GetAllAsync();
            if (orders.Count == 0)
            {
                return Ok(new ApiResponse(
                    HttpStatusCode.OK,
                    true,
                    "Không có đơn hàng nào cả.",
                    null
                ));
            }
            return Ok(new ApiResponse(
                    HttpStatusCode.OK,
                    true,
                    "Lấy dữ liệu thành công!",
                    orders
                ));
        }
        //Get by id
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetById(Guid orderId)
        {
            var orders = await _orderService.GetByIdAsync(orderId);
            if (orders == null)
            {
                return Ok(new ApiResponse(
                    HttpStatusCode.OK,
                    true,
                    "Không có đơn hàng nào có ID đó cả.",
                    null
                ));
            }
            return Ok(new ApiResponse(
                    HttpStatusCode.OK,
                    true,
                    "Lấy dữ liệu thành công!",
                    orders
                ));
        }
        ////Create
        [HttpPost]
        public async Task<IActionResult> AddOrder([FromBody] OrderRequest order)
        {
            if (order == null)
            {
                return BadRequest(new { message = "Invalid orders data" });
            }

            var createdOrder = await _orderService.CreateAsync(order);
            return Ok(createdOrder);
        }
        //Delete by id
        [HttpDelete("{orderId}")]
        public async Task<IActionResult> DeleteOrder(Guid orderId)
        {
            var result = await _orderService.DeleteByIdAsync(orderId);

            if (!result)
            {
                return NotFound(new { message = "Order not found" });
            }

            return Ok(new { message = "Order deleted successfully" });
        }
    }
}
