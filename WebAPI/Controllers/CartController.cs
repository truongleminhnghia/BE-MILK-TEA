using System.Net;
using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Business_Logic_Layer.Services;
using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
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
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public CartController(ICartService cartService, IMapper mapper, ApplicationDbContext context)
        {
            _cartService = cartService;
            _mapper = mapper;
            _context = context;
        }
        [HttpGet("{accountId}")]
        public async Task<IActionResult> GetCartDetails(Guid accountId)
        {
            if (accountId == Guid.Empty)
            {
                return BadRequest(new { message = "Account ID không hợp lệ!" });
            }

            var cart = await _context.Carts
                .Include(c => c.CartItems) // Lấy luôn danh sách CartItems
                .FirstOrDefaultAsync(c => c.AccountId == accountId);

            if (cart == null)
            {
                return NotFound(new { message = "Không tìm thấy giỏ hàng!" });
            }

            var cartDetails = cart.CartItems.Select(item => new
            {
                item.Id,
                item.IngredientProductId,
                item.Quantity,
                //item.Price
            }).ToList();

            return Ok(new
            {
                code = 200,
                message = "Lấy dữ liệu thành công!",
                success = true,
                data = new
                {
                    cartId = cart.Id,
                    accountId = cart.AccountId,
                    cartItems = cartDetails
                }
            });
        }
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

                if (result.HasValue && result.Value)
                    return Ok(new { message = "Xóa sản phẩm khỏi giỏ hàng thành công!" });

                return BadRequest(new { message = "Không thể xóa sản phẩm khỏi giỏ hàng!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống!", error = ex.Message });
            }
        }
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
    }
}
