
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
        private readonly IRedisService _redisCacheService;
        private const string CartItemCacheKey = "cart_item_cache";
        private const int CacheExpirationMinutes = 10;
        public CartItemController(ICartItemService cartItemService, IRedisService redisCacheService)
        {
            _cartItemService = cartItemService;
            _redisCacheService = redisCacheService;
        }

        [HttpPost]
        [Authorize(Roles = "ROLE_CUSTOMER")]
        public async Task<IActionResult> CreateCartItem([FromBody] CartItemRequest request)
        {
            try
            {
                var result = await _cartItemService.CreateCartItem(request);
                await _redisCacheService.RemoveByPrefixAsync(CartItemCacheKey);
                return Ok(new ApiResponse((int)HttpStatusCode.OK, true, "Thêm sản phẩm vào giỏ hàng thành công", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, false, ex.Message));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ROLE_CUSTOMER")]
        public async Task<IActionResult> DeleteCartItem([FromRoute] Guid id)
        {
            try
            {
                var success = await _cartItemService.Delete(id);
                if (!success)
                {
                    return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, false, "Không tìm thấy sản phẩm trong giỏ hàng"));
                }
                await _redisCacheService.RemoveByPrefixAsync(CartItemCacheKey);
                await _redisCacheService.RemoveAsync($"{CartItemCacheKey}:{id}");

                return Ok(new ApiResponse((int)HttpStatusCode.OK, true, "Xóa sản phẩm khỏi giỏ hàng thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, false, ex.Message));
            }
        }

        [HttpGet("{id}")]
        //[Authorize(Roles = "ROLE_CUSTOMER")]
        public async Task<IActionResult> GetCartItemById([FromRoute] Guid id)
        {
            try
            {
                var cacheKey = $"{CartItemCacheKey}:{id}";
                var cachedCategory = await _redisCacheService.GetAsync<CartItemResponse>(cacheKey);

                if (cachedCategory != null)
                {
                    return Ok(new ApiResponse(HttpStatusCode.OK.GetHashCode(), true, "Thành công (from cache)", cachedCategory));
                }
                var result = await _cartItemService.GetById(id);
                await _redisCacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(CacheExpirationMinutes));
                return Ok(new ApiResponse((int)HttpStatusCode.OK, true, "Lấy thông tin sản phẩm thành công", result));
            }
            catch (Exception ex)
            {
                return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, false, ex.Message));
            }
        }

        [HttpGet("cart/{cartId}")]
        //[Authorize(Roles = "ROLE_CUSTOMER")]
        public async Task<IActionResult> GetCartItemsByCartId([FromRoute] Guid cartId)
        {
            try
            {
                var cacheKey = $"{CartItemCacheKey}:{cartId}";
                var cachedCategory = await _redisCacheService.GetAsync<IEnumerable<CartItemResponse>>(cacheKey);

                if (cachedCategory != null)
                {
                    return Ok(new ApiResponse(HttpStatusCode.OK.GetHashCode(), true, "Thành công (from cache)", cachedCategory));
                }
                var result = await _cartItemService.GetByCart(cartId);
                if (result != null)
                {
                    await _redisCacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(CacheExpirationMinutes));
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
        [Authorize(Roles = "ROLE_CUSTOMER")]
        public async Task<IActionResult> UpdateCartItem([FromRoute] Guid id, [FromBody] UpdateCartItemRequest request)
        {
            try
            {
                var success = await _cartItemService.UpdateCartItem(id, request);
                if (!success)
                {
                    return NotFound(new ApiResponse((int)HttpStatusCode.NotFound, false, "Cập nhật thất bại, không tìm thấy sản phẩm trong giỏ hàng"));
                }
                await _redisCacheService.RemoveAsync($"{CartItemCacheKey}:{id}");
                await _redisCacheService.RemoveByPrefixAsync(CartItemCacheKey);
                return Ok(new ApiResponse((int)HttpStatusCode.OK, true, "Cập nhật sản phẩm trong giỏ hàng thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse((int)HttpStatusCode.BadRequest, false, ex.Message));
            }
        }
    }
}
