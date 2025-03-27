
using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Business_Logic_Layer.Services;
using Business_Logic_Layer.Services.Carts;
using Data_Access_Layer.Entities;
using Microsoft.AspNetCore.Authorization;
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

        [HttpPost]
        [Authorize("ROLE_CUSTOMER")]
        public async Task<IActionResult> CreateCartItem([FromBody] CartItemRequest request)
        {
            try
            {
                var result = await _cartItemService.CreateCartItem(request);
                return Ok(new ApiResponse((int)HttpStatusCode.OK, true, "Thêm sản phẩm vào giỏ hàng thành công", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, false, ex.Message));
            }
        }

        [HttpDelete("{id}")]
        [Authorize("ROLE_CUSTOMER")]
        public async Task<IActionResult> DeleteCartItem([FromRoute] Guid id)
        {
            try
            {
                var success = await _cartItemService.Delete(id);
                if (!success)
                {
                    return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, false, "Không tìm thấy sản phẩm trong giỏ hàng"));
                }
                return Ok(new ApiResponse((int)HttpStatusCode.OK, true, "Xóa sản phẩm khỏi giỏ hàng thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, false, ex.Message));
            }
        }

        [HttpGet("{id}")]
        [Authorize("ROLE_CUSTOMER")]
        public async Task<IActionResult> GetCartItemById([FromRoute] Guid id)
        {
            try
            {
                var result = await _cartItemService.GetById(id);
                return Ok(new ApiResponse((int)HttpStatusCode.OK, true, "Lấy thông tin sản phẩm thành công", result));
            }
            catch (Exception ex)
            {
                return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, false, ex.Message));
            }
        }

        [HttpGet("cart/{cartId}")]
        [Authorize("ROLE_CUSTOMER")]
        public async Task<IActionResult> GetCartItemsByCartId([FromRoute] Guid cartId)
        {
            try
            {
                var result = await _cartItemService.GetByCart(cartId);
                if (result == null)
                {
                    return Ok(new ApiResponse((int)HttpStatusCode.OK, true, "Lấy danh sách sản phẩm trong giỏ hàng thành công", result));
                }
                return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, false, "danh sach khong ton tai"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, false, ex.Message));
            }
        }

        [HttpPut("{id}")]
        [Authorize("ROLE_CUSTOMER")]
        public async Task<IActionResult> UpdateCartItem([FromRoute] Guid id, [FromBody] UpdateCartItemRequest request)
        {
            try
            {
                var success = await _cartItemService.UpdateCartItem(id, request);
                if (!success)
                {
                    return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, false, "Cập nhật thất bại, không tìm thấy sản phẩm trong giỏ hàng"));
                }
                return Ok(new ApiResponse((int)HttpStatusCode.OK, true, "Cập nhật sản phẩm trong giỏ hàng thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, false, ex.Message));
            }
        }
    }
}
