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
using Org.BouncyCastle.Asn1.Crmf;


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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var carts = await _cartService.GetByIdAsync(id);
            if (carts == null)
            {
                return Ok(new ApiResponse(
                    ((int)HttpStatusCode.OK),
                    true,
                    "Không có cart nào có ID đó cả.",
                    null
                ));
            }
            return Ok(new ApiResponse(
                    ((int)HttpStatusCode.OK),
                    true,
                    "Lấy dữ liệu thành công!",
                    carts
                ));
        }
        //[HttpGet("{accountId}")]
        //public async Task<IActionResult> GetCartDetails(Guid accountId)
        //{
        //    if (accountId == Guid.Empty)
        //    {
        //        return BadRequest(new { message = "Account ID không hợp lệ!" });
        //    }

        //    var cart = await _cartService.GetByIdAsync(accountId);

        //    if (cart == null)
        //    {
        //        return NotFound(new { message = "Không tìm thấy giỏ hàng!" });
        //    }

        //    return Ok(new
        //    {
        //        code = 200,
        //        message = cart.CartItems.Any() ? "Lấy dữ liệu thành công!" : "Giỏ hàng của bạn hiện đang trống.",
        //        success = true,
        //        data = new
        //        {
        //            cartId = cart.CartId,
        //            accountId = cart.AccountId,
        //            cartItems = cart.CartItems
        //        }
        //    });
        //}

        //Create
        [HttpPost]
        public async Task<IActionResult> AddCart([FromBody] CartRequest cart)
        {
            if (cart == null)
            {
                return BadRequest(new { message = "Invalid cart data" });
            }

            var createCart = await _cartService.CreateAsync(cart);
            return Ok(createCart);
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
