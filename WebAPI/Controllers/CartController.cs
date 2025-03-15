using System.Net;
using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Business_Logic_Layer.Services;
using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace WebAPI.Controllers
{
    [Route("api/v1/carts")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ICartRepository _cartRepository;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public CartController(ICartService cartService, IMapper mapper, ApplicationDbContext context, ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
            _cartService = cartService;
            _mapper = mapper;
            _context = context;
        }
        //get cart
        [HttpGet("{accountId}")]
        public async Task<IActionResult> GetCartDetails(Guid accountId)
        {
            if (accountId == Guid.Empty)
            {
                return BadRequest(new { message = "Account ID không hợp lệ!" });
            }

            var cart = await _cartService.GetByIdAsync(accountId);

            if (cart == null)
            {
                return NotFound(new { message = "Không tìm thấy giỏ hàng!" });
            }

            return Ok(new
            {
                code = 200,
                message = cart.CartItems.Any() ? "Lấy dữ liệu thành công!" : "Giỏ hàng của bạn hiện đang trống.",
                success = true,
                data = new
                {
                    cartId = cart.CartId,
                    accountId = cart.AccountId,
                    cartItems = cart.CartItems
                }
            });
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

        //clear cart
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart([FromQuery] Guid accountId)
        {
            if (accountId == Guid.Empty)
            {
                return BadRequest(new { message = "Account ID không hợp lệ!" });
            }

            var success = await _cartService.ClearCartAsync(accountId);
            if (!success)
            {
                return NotFound(new { message = "Giỏ hàng đã trống hoặc không tồn tại!" });
            }

            return Ok(new { message = "Giỏ hàng đã được xóa toàn bộ item thành công!" });
        }
    }
}
