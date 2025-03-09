using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Services;
using Data_Access_Layer.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Presentation_Layer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionDetailController : ControllerBase
    {
        private readonly IPromotionDetailService _service;

        public PromotionDetailController(IPromotionDetailService service)
        {
            _service = service;
        }

        // ✅ Lấy tất cả PromotionDetail
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PromotionDetail>>> GetAll()
        {
            var result = await _service.GetAll();
            return Ok(result);
        }

        // ✅ Lấy chi tiết PromotionDetail theo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<PromotionDetail>> GetById(Guid id)
        {
            //var promotionDetail = await _service.GetById(id);
            //if (promotionDetail == null)
            //{
            //    return NotFound(new { message = "Promotion detail not found." });
            //}
            //return Ok(promotionDetail);
            var result = await _service.GetById(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // ✅ Tạo mới PromotionDetail
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PromotionDetailRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var promotionDetail = new PromotionDetail
            {
                PromotionId = request.PromotionId ?? Guid.NewGuid(),
                PromotionName = request.PromotionName,
                Description = request.Description,
                DiscountValue = request.DiscountValue,
                MiniValue = request.MiniValue,
                MaxValue = request.MaxValue
            };

            await _service.Create(promotionDetail);
            return Ok(new { message = "Promotion đã tạo thành công" });
        }

        // ✅ Cập nhật PromotionDetail theo ID
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] PromotionDetail promotionDetail)
        {
            if (promotionDetail == null || id != promotionDetail.Id)
            {
                return BadRequest(new { message = "Invalid promotion detail data." });
            }

            var existingPromotion = await _service.GetById(id);
            if (existingPromotion == null)
            {
                return NotFound(new { message = "Không tìm thấy chi tiết Promotion." });
            }

            await _service.Update(promotionDetail);
            return NoContent();
        }

        // ✅ Xóa PromotionDetail theo ID
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var existingPromotion = await _service.GetById(id);
            if (existingPromotion == null)
            {
                return NotFound(new { message = "Promotion detail not found." });
            }

            await _service.Delete(id);
            return NoContent();
        }
    }
}
