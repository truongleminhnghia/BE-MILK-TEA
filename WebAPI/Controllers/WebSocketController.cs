using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Business_Logic_Layer.Services;
using Data_Access_Layer.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/ws")]
    public class WebSocketController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public WebSocketController(ICategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategoriesWebSocket(
            [FromQuery] CategoryStatus? categoryStatus,
            [FromQuery] CategoryType? categoryType,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool isDescending = false,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null
        )
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

                try
                {
                    while (webSocket.State == WebSocketState.Open)
                    {
                        var categories = await _categoryService.GetAllCategoriesAsync(
                                search, sortBy, isDescending, categoryStatus, categoryType, startDate, endDate, page, pageSize);
                        var categoryRes = _mapper.Map<IEnumerable<CategoryResponse>>(categories);
                        string jsonString = JsonSerializer.Serialize(categoryRes);
                        var buffer = Encoding.UTF8.GetBytes(jsonString);
                        await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                        await Task.Delay(2000);
                    }
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Conneciton close by the server", CancellationToken.None);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"WebSocket error: {ex.Message}");
                }
            }
            else
            {
                return BadRequest("WebSocket request expected.");
            }

            return Ok();
        }

    }
}