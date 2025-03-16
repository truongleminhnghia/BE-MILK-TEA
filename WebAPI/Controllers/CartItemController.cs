using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/v1/cart-items")]
    [ApiController]
    public class CartItemController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IMapper _mapper;
        public CartItemController(ICartService cartService, IMapper mapper)
        {
            _cartService = cartService;
            _mapper = mapper;
        }

        //add item to cart
        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            if (request == null || request.AccountId == Guid.Empty || request.IngredientProductId == Guid.Empty || request.Quantity <= 0)
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ!" });
            }

            try
            {
                // Gọi phương thức lưu dữ liệu mà không cần gán kết quả
                await _cartService.AddToCartAsync(request.AccountId, request.IngredientProductId, request.Quantity);

                return Ok(new { message = "Thành công cho item vào cart!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi add to cart", error = ex.Message });
            }
        }

        //update quantity of cart item
        [HttpPut("update")]
        public async Task<IActionResult> UpdateCartItem([FromBody] UpdateCartItemRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _cartService.UpdateCartItemQuantityAsync(request.AccountId, request.IngredientProductId, request.Quantity);
            if (!success)
            {
                return NotFound(new { message = "Sản phẩm không tồn tại trong giỏ hàng!" });
            }

            return Ok(new { message = "Cập nhật số lượng thành công!" });
        }

        //remove item from cart
        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveFromCart([FromBody] RemoveCartItemRequest request)
        {
            if (request == null || request.AccountId == Guid.Empty || request.IngredientProductId == Guid.Empty)
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ!" });
            }

            try
            {
                var result = await _cartService.RemoveItemAsync(request.AccountId, request.IngredientProductId);

                if (result)
                {
                    return Ok(new { message = "Xóa sản phẩm thành công!" });
                }

                return BadRequest(new { message = "Không thể xóa sản phẩm khỏi giỏ hàng!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống!", error = ex.Message });
            }
        }
    
    }
}
