using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Business_Logic_Layer.Services.Interfaces;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using System.Net;
using Business_Logic_Layer.Services.Cart;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCart([FromBody] CartRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new ApiResponse(
                        HttpStatusCode.BadRequest.GetHashCode(),
                        false,
                        "Dữ liệu không hợp lệ"
                    ));
                }

                var result = await _cartService.CreateCart(request);
                if (!result)
                {
                    return StatusCode(500, new ApiResponse(
                        HttpStatusCode.InternalServerError.GetHashCode(),
                        false,
                        "Lỗi khi tạo giỏ hàng"
                    ));
                }

                return Ok(new ApiResponse(
                    HttpStatusCode.OK.GetHashCode(),
                    true,
                    "Tạo giỏ hàng thành công"
                ));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(
                    HttpStatusCode.InternalServerError.GetHashCode(),
                    false,
                    $"Error: {ex.Message}"
                ));
            }
        }
    }
}
