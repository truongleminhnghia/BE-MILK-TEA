using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Business_Logic_Layer.Services.Interfaces;
using Data_Access_Layer.Entities;
using Business_Logic_Layer.Services;
using Business_Logic_Layer.Models.Responses;
using System.Net;


namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartItemController : ControllerBase
    {
        private readonly ICartItemService _cartItemService;

        public CartItemController(ICartItemService cartItemService)
        {
            _cartItemService = cartItemService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _cartItemService.GetAll();
                return Ok(new ApiResponse(
                    HttpStatusCode.OK.GetHashCode(),
                    true,
                    "Lấy danh sách CartItem thành công",
                    result
                ));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(
                    HttpStatusCode.InternalServerError.GetHashCode(),
                    false,
                    $"Lỗi khi lấy danh sách CartItem: {ex.Message}"
                ));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var result = await _cartItemService.GetById(id);
                if (result == null)
                {
                    return NotFound(new ApiResponse(
                        HttpStatusCode.NotFound.GetHashCode(),
                        false,
                        $"Không tìm thấy CartItem với ID {id}"
                    ));
                }
                return Ok(new ApiResponse(
                    HttpStatusCode.OK.GetHashCode(),
                    true,
                    "Lấy thông tin CartItem thành công",
                    result
                ));

            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(
                    HttpStatusCode.InternalServerError.GetHashCode(),
                    false,
                    $"Lỗi khi lấy CartItem: {ex.Message}"
                ));
            }

        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CartItem cartItem)
        {
            try
            {
                if (cartItem == null)
                {
                    return BadRequest(new ApiResponse(
                        HttpStatusCode.BadRequest.GetHashCode(),
                        false,
                        "Dữ liệu CartItem không hợp lệ"
                    ));
                }

                var result = await _cartItemService.Create(cartItem);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, new ApiResponse(
                    HttpStatusCode.Created.GetHashCode(),
                    true,
                    "Tạo CartItem thành công",
                    result
                ));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(
                    HttpStatusCode.InternalServerError.GetHashCode(),
                    false,
                    $"Lỗi khi tạo CartItem: {ex.Message}"
                ));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CartItem cartItem)
        {
            try
            {
                if (cartItem == null || id != cartItem.Id)
                {
                    return BadRequest(new ApiResponse(
                        HttpStatusCode.BadRequest.GetHashCode(),
                        false,
                        "Dữ liệu không hợp lệ hoặc ID không khớp"
                    ));
                }

                var result = await _cartItemService.Update(cartItem);
                if (result == null)
                {
                    return NotFound(new ApiResponse(
                        HttpStatusCode.NotFound.GetHashCode(),
                        false,
                        $"Không tìm thấy CartItem với ID {id}"
                    ));
                }

                return Ok(new ApiResponse(
                    HttpStatusCode.OK.GetHashCode(),
                    true,
                    "Cập nhật CartItem thành công",
                    result
                ));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(
                    HttpStatusCode.InternalServerError.GetHashCode(),
                    false,
                    $"Lỗi khi cập nhật CartItem: {ex.Message}"
                ));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _cartItemService.Delete(id);
                if (!result)
                {
                    return NotFound(new ApiResponse(
                        HttpStatusCode.NotFound.GetHashCode(),
                        false,
                        $"Không tìm thấy CartItem với ID {id}"
                    ));
                }

                return Ok(new ApiResponse(
                    HttpStatusCode.OK.GetHashCode(),
                    true,
                    "Xóa CartItem thành công"
                ));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(
                    HttpStatusCode.InternalServerError.GetHashCode(),
                    false,
                    $"Lỗi khi xóa CartItem: {ex.Message}"
                ));
            }
        }
    }
}
