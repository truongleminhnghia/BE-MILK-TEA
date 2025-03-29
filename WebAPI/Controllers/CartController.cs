using System.Globalization;
using System.Net;
using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Business_Logic_Layer.Services;
using Business_Logic_Layer.Services.Carts;
using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;
using Data_Access_Layer.Repositories;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IRedisService _redisCacheService;
        private const string CartCacheKey = "cart_cache";
        private const int CacheExpirationMinutes = 10;
        public CartController(ICartService cartService, IRedisService redisCacheService)
        {
            _cartService = cartService;
            _redisCacheService = redisCacheService;
        }

        [HttpPost("{accountId}")]
        [Authorize(Roles = "ROLE_CUSTOMER")]
        public async Task<IActionResult> CreateCart(Guid accountId)
        {
            try
            {
                var cart = await _cartService.CreateCart(accountId);
                await _redisCacheService.RemoveByPrefixAsync(CartCacheKey);
                return Ok(new ApiResponse(201, true, "Tạo giỏ hàng thành công", cart));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse(404, false, ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse(400, false, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, false, "Lỗi server", ex.Message));
            }
        }

        // [HttpGet("{id}")]
        // public async Task<IActionResult> GetById(Guid id)
        // {
        //     try
        //     {
        //         var cart = await _cartService.GetById(id);
        //         if (cart == null)
        //         {
        //             return NotFound(new ApiResponse(404, false, "Giỏ hàng không tồn tại"));
        //         }
        //         return Ok(new ApiResponse(200, true, "Lấy giỏ hàng thành công", cart));
        //     }
        //     catch (ArgumentException ex)
        //     {
        //         return BadRequest(new ApiResponse(400, false, ex.Message));
        //     }
        //     catch (Exception ex)
        //     {
        //         return StatusCode(500, new ApiResponse(500, false, "Lỗi server", ex.Message));
        //     }
        // }

        [HttpGet("{accountId}")]
        [Authorize(Roles = "ROLE_CUSTOMER")]
        public async Task<IActionResult> GetByAccount(Guid accountId)
        {
            try
            {
                // Generate a unique cache key based on all parameters
                var cacheKey = $"{CartCacheKey}:{accountId}";
                // Try to get data from cache first
                var cachedData = await _redisCacheService.GetAsync<CartResponse>(cacheKey);
                if (cachedData != null)
                {
                    return Ok(new ApiResponse(HttpStatusCode.OK.GetHashCode(), true, "Thành công (from cache)", cachedData));
                }
                var cart = await _cartService.GetByAccount(accountId);
                await _redisCacheService.SetAsync(cacheKey, cart, TimeSpan.FromMinutes(CacheExpirationMinutes));
                return Ok(new ApiResponse(200, true, "Lấy giỏ hàng thành công", cart));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse(404, false, ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse(400, false, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, false, "Lỗi server", ex.Message));
            }
        }

    }
}
