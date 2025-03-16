using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AutoMapper;
using Azure;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Business_Logic_Layer.Services;
using Business_Logic_Layer.Services.DashboardService;
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
        private readonly IRecipeService _recipeService;
        private readonly IDashboardService _dashboardService;
        private readonly IAccountService _accountService;

        public WebSocketController(ICategoryService categoryService, IIngredientService ingredientService, IMapper mapper, IPromotionService promotionService, IRecipeService recipeService, IDashboardService dashboardService, IAccountService accountService)
        {
            _promotionService = promotionService;
            _ingredientService = ingredientService;
            _categoryService = categoryService;
            _mapper = mapper;
            _recipeService = recipeService;
            _dashboardService = dashboardService;
            _accountService = accountService;
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


        [HttpGet("/recipes")]
        public async Task<IActionResult> GetRecipesWebSocket(
            [FromQuery] RecipeStatusEnum? recipeStatus,
            [FromQuery] Guid? categoryId,
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
                        var recipes = await _recipeService.GetAllRecipesAsync(
                                search, sortBy, isDescending, recipeStatus, categoryId, startDate, endDate, page, pageSize);

                        var recipeRes = _mapper.Map<IEnumerable<RecipeResponse>>(recipes);
                        string jsonString = JsonSerializer.Serialize(recipeRes);
                        var buffer = Encoding.UTF8.GetBytes(jsonString);
                        await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
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

        [HttpGet("/dashboard")]
        public async Task<IActionResult> GetDashboardData()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                try
                {
                    while (webSocket.State == WebSocketState.Open)
                    {
                        var dashboardData = await _dashboardService.GetDashboardDataAsync();
                        var response = new ApiResponse(HttpStatusCode.OK.GetHashCode(), true, "Thành công", dashboardData);
                        string jsonString = JsonSerializer.Serialize(response);
                        var buffer = Encoding.UTF8.GetBytes(jsonString);
                        await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                        await Task.Delay(2000); // Update every 2 seconds
                    }
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed by the server", CancellationToken.None);

                }
                catch (WebSocketException ex)
                {
                    Console.WriteLine($"WebSocket error: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            else
            {
                return BadRequest("WebSocket request expected.");
            }
            return Ok();

        }

        [HttpGet("/accounts")]
        public async Task<IActionResult> GetAccountsWebSocket(
            [FromQuery] string? search = null,
            [FromQuery] AccountStatus? accountStatus = null,
            [FromQuery] RoleName? roleName = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool isDescending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                try
                {
                    while (webSocket.State == WebSocketState.Open)
                    {
                        var accounts = await _accountService.GetAllAccountsAsync(
                            search, accountStatus, roleName, sortBy, isDescending, page, pageSize);

                        var accountResponses = _mapper.Map<IEnumerable<AccountResponse>>(accounts);
                        string jsonString = JsonSerializer.Serialize(accountResponses, new JsonSerializerOptions
                        {
                            WriteIndented = true,
                            Converters = { new JsonStringEnumConverter() }
                        });

                        var buffer = Encoding.UTF8.GetBytes(jsonString);
                        await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
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



    }
}