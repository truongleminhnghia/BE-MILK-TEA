using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Business_Logic_Layer.Services.IngredientReviewService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/v1/ingredientReviews")]
    [ApiController]
    public class IngredientReviewController : ControllerBase
    {
        private readonly IIngredientReviewService _ingredientReviewService;

        public IngredientReviewController(IIngredientReviewService ingredientReviewService)
        {
            _ingredientReviewService = ingredientReviewService;
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var review = await _ingredientReviewService.GetByIdAsync(id);
            if (review == null)
                return NotFound();

            return Ok(review);
        }

        // GET bằng IngredientId
        [HttpGet("ingredientId")]
        public async Task<IActionResult> GetByIngredientId(Guid ingredientId)
        {
            var reviews = await _ingredientReviewService.GetByIngredientIdAsync(ingredientId);
            return Ok(reviews);
        }

        //GET bằng AccountId
        [HttpGet("accountId")]
        public async Task<IActionResult> GetByAccountId(Guid accountId)
        {
            var reviews = await _ingredientReviewService.GetByAccountIdAsync(accountId);
            return Ok(reviews);
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateIngredientReviewRequest request)
        {
            try
            {
                var createdReview = await _ingredientReviewService.CreateAsync(request);
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = createdReview.Id },
                    createdReview
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        // PUT
        [HttpPut("id")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateIngredientReviewRequest request
        )
        {
            try
            {
                var updatedReview = await _ingredientReviewService.UpdateAsync(id, request);
                if (updatedReview == null)
                    return NotFound();

                return Ok(updatedReview);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE
        [HttpDelete("id")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _ingredientReviewService.DeleteAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
