using Folio.Core.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace Folio.Infrastructure.Caching
{
    public class RedisCacheService : ICacheService
    {
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new(JsonSerializerDefaults.Web);
        private readonly IConnectionMultiplexer _redis;

        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            var db = _redis.GetDatabase();
            var value = await db.StringGetAsync(key);

            if (value.IsNullOrEmpty)
                return default;

            return JsonSerializer.Deserialize<T>(value.ToString(), _jsonSerializerOptions);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan timeSpan, CancellationToken cancellationToken = default)
        {
            var db = _redis.GetDatabase();
            var payload = JsonSerializer.Serialize(value, _jsonSerializerOptions);

            await db.StringSetAsync(key, payload, timeSpan);
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            var db = _redis.GetDatabase();
            await db.KeyDeleteAsync(key);
        }

        public async Task<long> IncrementAsync(string key, CancellationToken cancellationToken = default)
        {
            var db = _redis.GetDatabase();
            return await db.StringIncrementAsync(key);
        }
    }
}
