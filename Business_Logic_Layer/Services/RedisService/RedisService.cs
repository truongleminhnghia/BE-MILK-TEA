using StackExchange.Redis;
using System;
using System.Text.Json;
using System.Threading.Tasks;

public class RedisService : IRedisService
{
    private readonly IConnectionMultiplexer _redisConnection;

    public RedisService(IConnectionMultiplexer redisConnection)
    {
        _redisConnection = redisConnection;
    }

    public async Task<T> GetAsync<T>(string key)
    {
        var db = _redisConnection.GetDatabase();
        var value = await db.StringGetAsync(key);
        return value.HasValue ? JsonSerializer.Deserialize<T>(value) : default;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan expiry)
    {
        var db = _redisConnection.GetDatabase();
        var serializedValue = JsonSerializer.Serialize(value);
        await db.StringSetAsync(key, serializedValue, expiry);
    }

    public async Task RemoveAsync(string key)
    {
        var db = _redisConnection.GetDatabase();
        await db.KeyDeleteAsync(key);
    }

    public async Task RemoveByPrefixAsync(string prefix)
    {
        var server = _redisConnection.GetServer(_redisConnection.GetEndPoints()[0]);
        var keys = server.Keys(pattern: $"{prefix}*");
        var db = _redisConnection.GetDatabase();

        foreach (var key in keys)
        {
            await db.KeyDeleteAsync(key);
        }
    }

    public async Task<bool> ExistsAsync(string key)
    {
        var db = _redisConnection.GetDatabase();
        return await db.KeyExistsAsync(key);
    }
}