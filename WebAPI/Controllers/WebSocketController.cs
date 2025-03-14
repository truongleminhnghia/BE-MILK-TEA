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
using Business_Logic_Layer.Services.IngredientService;
using Business_Logic_Layer.Services.PromotionService;
using Data_Access_Layer.Enum;
using Data_Access_Layer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/ws")]
    public class WebSocketController : ControllerBase
    {
        private readonly IIngredientService _ingredientService;
        private readonly ICategoryService _categoryService;
        private readonly IPromotionService _promotionService;
        private readonly IMapper _mapper;

        public WebSocketController(ICategoryService categoryService, IIngredientService ingredientService, IMapper mapper, IPromotionService promotionService)
        {
            _promotionService = promotionService;
            _ingredientService = ingredientService;
            _categoryService = categoryService;
            _mapper = mapper;
        }

        [HttpGet("ingredients")]
        public async Task<IActionResult> GetIngredientsWebSocket(
                        [FromQuery] string? search,
                        [FromQuery] string? categorySearch,
                        [FromQuery] Guid? categoryId,
                        [FromQuery] string? sortBy,
                        [FromQuery] DateTime? startDate,
                        [FromQuery] DateTime? endDate,
                        [FromQuery] IngredientStatus? status,
                        [FromQuery] decimal? minPrice,
                        [FromQuery] decimal? maxPrice,
                        [FromQuery] bool? isSale,
                        [FromQuery] bool isDescending = false,
                        [FromQuery] int pageCurrent = 1,
                        [FromQuery] int pageSize = 10)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                try
                {
                    while (webSocket.State == WebSocketState.Open)
                    {
                        var ingredients = await _ingredientService.GetAllAsync(
                            search, categorySearch, categoryId, sortBy, isDescending,
                            pageCurrent, pageSize, startDate, endDate, status, minPrice, maxPrice, isSale
                        );

                        var ingredientResponses = _mapper.Map<IEnumerable<IngredientResponse>>(ingredients);
                        string jsonString = JsonSerializer.Serialize(ingredientResponses);
                        var buffer = Encoding.UTF8.GetBytes(jsonString);

                        await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

                        // Gửi dữ liệu mỗi 2 giây
                        await Task.Delay(2000);
                    }
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed by the server", CancellationToken.None);
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

        [HttpGet("promotion")]
        public async Task<IActionResult> GetPromotionsWebSocket()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                try
                {
                    string lastSentData = string.Empty;

                    while (webSocket.State == WebSocketState.Open)
                    {
                        var promotions = await _promotionService.GetAllPromotionAsync(
                            isActive: true, search: null, sortBy: null,
                            isDescending: false, promotionType: null,
                            startDate: null, endDate: null, page: 1, pageSize: 10
                        );

                        var promoRes = _mapper.Map<IEnumerable<PromotionResponse>>(promotions);
                        string jsonString = JsonSerializer.Serialize(promoRes);

                        //  Chỉ gửi dữ liệu nếu có thay đổi
                        if (jsonString != lastSentData)
                        {
                            lastSentData = jsonString;
                            var buffer = Encoding.UTF8.GetBytes(jsonString);
                            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                        }

                        await Task.Delay(2000); // Kiểm tra dữ liệu mỗi 2 giây
                    }

                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed by server", CancellationToken.None);
                }
                catch (WebSocketException ex)
                {
                    Console.WriteLine($" WebSocket Error: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($" Error: {ex.Message}");
                }
            }
            else
            {
                return BadRequest("WebSocket request expected.");
            }

            return Ok();
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