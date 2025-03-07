using System.Net;
using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Business_Logic_Layer.Services;
using Data_Access_Layer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace WebAPI.Controllers
{
    [Route("api/v1/carts")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IMapper _mapper;

        public CartController(ICartService cartService, IMapper mapper)
        {
            _cartService = cartService;
            _mapper = mapper;
        }
        //Get by id
        [HttpGet("{cartId}")]
        public async Task<IActionResult> GetById(Guid cartId)
        {
            var carts = await _cartService.GetByIdAsync(cartId);
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

        ////Create
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

        //UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCart(
        Guid id,
            [FromBody] CartRequest cartRequest
        )
        {
            if (cartRequest == null)
            {
                return BadRequest(new { message = "Invalid cart data" });
            }

            var cart = _mapper.Map<Cart>(cartRequest);
            var updatedCart = await _cartService.UpdateAsync(id, cart);

            if (updatedCart == null)
            {
                return NotFound(new { message = "Cart not found" });
            }

            return Ok(updatedCart);
        }
    }
}
