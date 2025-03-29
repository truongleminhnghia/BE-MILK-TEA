using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Business_Logic_Layer.Services.IngredientReviewService;
using Data_Access_Layer.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Net;

namespace WebAPI.Controllers
{
    [Route("api/v1/ingredientReviews")]
    [ApiController]
    public class IngredientReviewController : ControllerBase
    {
        private readonly IIngredientReviewService _ingredientReviewService;
        private readonly IRedisService _redisCacheService;
        private const string IngredientReviewCacheKey = "ingredient_review_cache";
        private const int CacheExpirationMinutes = 10;

        public IngredientReviewController(IIngredientReviewService ingredientReviewService, IRedisService redisCacheService)
        {
            _ingredientReviewService = ingredientReviewService;
            _redisCacheService = redisCacheService;
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetById(Guid id)
        {
            // Generate a unique cache key based on all parameters
            var cacheKey = $"{IngredientReviewCacheKey}:{id}";
            // Try to get data from cache first
            var cachedData = await _redisCacheService.GetAsync<PageResult<CategoryResponse>>(cacheKey);
            if (cachedData != null)
            {
                return Ok(new ApiResponse(HttpStatusCode.OK.GetHashCode(), true, "Thành công (from cache)", cachedData));
            }
            var review = await _ingredientReviewService.GetByIdAsync(id);
            if (review == null)
                return NotFound();
            await _redisCacheService.SetAsync(cacheKey, review, TimeSpan.FromMinutes(CacheExpirationMinutes));
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
