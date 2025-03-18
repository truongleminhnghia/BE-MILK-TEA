using Business_Logic_Layer.Models.Responses;
using Business_Logic_Layer.Services;
using Data_Access_Layer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebAPI.Controllers
{
    [Route("api/v1/cart-items")]
    [ApiController]
    public class CartItemController : ControllerBase
    {
        private readonly ICartItemService _cartItemService;

        public CartItemController(ICartItemService cartItemService)
        {
            _cartItemService = cartItemService;
        }

        [HttpGet("by-cart/{cartId}")]
        public async Task<IActionResult> GetByCartId(Guid cartId)
        {
            try
            {
                var cartItems = await _cartItemService.GetByCartIdAsync(cartId);
                if (cartItems == null || !cartItems.Any())
                {
                    return NotFound( new ApiResponse (HttpStatusCode.NotFound.GetHashCode(),false, "Không tìm thấy danh sách cho giỏ hàng này."));
                }
                return Ok(new ApiResponse(
                    HttpStatusCode.OK.GetHashCode(),
                    true,
                    "Lấy danh sách danh sách cho giỏ hàng này thành công",
                    cartItems
                ));
            }
            catch(Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(HttpStatusCode.InternalServerError.GetHashCode(), false, "Đã có lỗi khi lấy giỏ hàng theo CartId: " + e.Message));
            }
            
        }
    }
}
