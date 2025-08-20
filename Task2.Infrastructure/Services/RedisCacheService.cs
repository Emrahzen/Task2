using StackExchange.Redis;
using System.Text.Json;
using Task2.Core.Interfaces;

namespace Task2.Infrastructure.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;
        private readonly bool _isConnected;

        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _database = redis.GetDatabase();
            _isConnected = _redis.IsConnected;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                if (!_isConnected) return default;
                
                var value = await _database.StringGetAsync(key);
                if (value.IsNull)
                    return default;

                return JsonSerializer.Deserialize<T>(value!);
            }
            catch (RedisConnectionException)
            {
                return default;
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            try
            {
                if (!_isConnected) return;
                
                var serializedValue = JsonSerializer.Serialize(value);
                await _database.StringSetAsync(key, serializedValue, expiration);
            }
            catch (RedisConnectionException)
            {
            }
            catch (Exception)
            {
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                if (!_isConnected) return;
                
                await _database.KeyDeleteAsync(key);
            }
            catch (RedisConnectionException)
            {
            }
            catch (Exception)
            {
            }
        }

        public async Task RemoveByPatternAsync(string pattern)
        {
            try
            {
                if (!_isConnected) return;
                
                var server = _redis.GetServer(_redis.GetEndPoints().First());
                var keys = server.Keys(pattern: pattern);
                
                foreach (var key in keys)
                {
                    await _database.KeyDeleteAsync(key);
                }
            }
            catch (RedisConnectionException)
            {
               
            }
            catch (Exception)
            {
                
            }
        }

        public async Task<bool> ExistsAsync(string key)
        {
            try
            {
                if (!_isConnected) return false;
                
                return await _database.KeyExistsAsync(key);
            }
            catch (RedisConnectionException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
} 