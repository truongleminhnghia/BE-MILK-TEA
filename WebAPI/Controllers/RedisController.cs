using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

[ApiController]
[Route("api/v1/redis")]
public class RedisController : ControllerBase
{
    private readonly RedisService _redisService;

    public RedisController(RedisService redisService)
    {
        _redisService = redisService;
    }

    // POST api/v1/redis/save
    [HttpPost("save")]
    public async Task<IActionResult> SaveValue([FromBody] RedisTestRequest request)
    {
        try
        {
            await _redisService.SetAsync(request.Key, request.Value, TimeSpan.FromMinutes(request.ExpiryMinutes));
            return Ok(new
            {
                Success = true,
                Message = "Value saved successfully",
                Key = request.Key,
                ExpiresInMinutes = request.ExpiryMinutes
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Success = false,
                Message = "Failed to save value",
                Error = ex.Message
            });
        }
    }

    // GET api/v1/redis/get/{key}
    [HttpGet("get/{key}")]
    public async Task<IActionResult> GetValue(string key)
    {
        try
        {
            var value = await _redisService.GetAsync<string>(key);

            if (value == null)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = "Key not found"
                });
            }

            return Ok(new
            {
                Success = true,
                Key = key,
                Value = value
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Success = false,
                Message = "Failed to retrieve value",
                Error = ex.Message
            });
        }
    }

    // DELETE api/v1/redis/remove/{key}
    [HttpDelete("remove/{key}")]
    public async Task<IActionResult> RemoveValue(string key)
    {
        try
        {
            var existed = await _redisService.ExistsAsync(key);
            await _redisService.RemoveAsync(key);

            return Ok(new
            {
                Success = true,
                Message = "Key deleted",
                KeyExisted = existed
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Success = false,
                Message = "Failed to remove key",
                Error = ex.Message
            });
        }
    }

    public class RedisTestRequest
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public int ExpiryMinutes { get; set; } = 10; // Default 10 minutes
    }
}