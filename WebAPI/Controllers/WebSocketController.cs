using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/ws")]
    public class WebSocketController : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                var random = new Random();

                try
                {
                    while (webSocket.State == WebSocketState.Open)
                    {
                        // Tạo giá trị x, y ngẫu nhiên
                        int x = random.Next(1, 100);
                        int y = random.Next(1, 100);
                        var message = $"{{ \"x\": {x}, \"y\": {y} }}";
                        var buffer = Encoding.UTF8.GetBytes(message);

                        // Gửi dữ liệu đến client
                        await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

                        // Chờ 2 giây trước khi gửi tiếp
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