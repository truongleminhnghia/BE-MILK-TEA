using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Business_Logic_Layer.Services.Interfaces;
using Data_Access_Layer.Entities;
using Business_Logic_Layer.Services;


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
            var result = await _cartItemService.GetAll();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _cartItemService.GetById(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CartItem cartItem)
        {
            if (cartItem == null) return BadRequest();
            var result = await _cartItemService.Create(cartItem);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CartItem cartItem)
        {
            if (cartItem == null || id != cartItem.Id) return BadRequest();
            var result = await _cartItemService.Update(cartItem);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _cartItemService.Delete(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
