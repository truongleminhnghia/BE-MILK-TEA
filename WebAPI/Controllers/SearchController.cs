using Business_Logic_Layer.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/v1/search")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet("suggest")]
        public IActionResult Suggest([FromQuery] string keyword, [FromQuery] int pageSize = 5)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest("Search keyword cannot be empty.");
            }

            var suggestions = _searchService.GetSuggestions(keyword, pageSize);
            return Ok(new { items = suggestions });
        }
    }
}
