public interface IRedisService
{
    Task<T> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan expiry);
    Task RemoveAsync(string key);
    Task RemoveByPrefixAsync(string prefix);
    Task<bool> ExistsAsync(string key);
}