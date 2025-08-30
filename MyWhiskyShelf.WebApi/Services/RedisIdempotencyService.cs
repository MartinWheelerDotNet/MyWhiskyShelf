using System.Text.Json;
using MyWhiskyShelf.WebApi.Interfaces;
using MyWhiskyShelf.WebApi.Models;
using StackExchange.Redis;

namespace MyWhiskyShelf.WebApi.Services;

public class RedisIdempotencyService(IConnectionMultiplexer connectionMultiplexer) : IIdempotencyService
{
    private readonly IDatabase _database = connectionMultiplexer.GetDatabase();

    public async Task<CachedResponse?> TryGetCachedResultAsync(Guid idempotencyKey)
    {
        var cachedResultString = await _database.StringGetAsync(idempotencyKey.ToString());
        return cachedResultString.IsNullOrEmpty
            ? null
            : JsonSerializer.Deserialize<CachedResponse>(cachedResultString!);
    }

    public async Task AddToCacheAsync(
        string idempotencyKey,
        int statusCode,
        string content,
        string? contentType,
        Dictionary<string, string?[]> headers)
    {
        var cachedResponse = new CachedResponse(statusCode, content, contentType, headers);
        await _database.StringSetAsync(idempotencyKey, JsonSerializer.SerializeToUtf8Bytes(cachedResponse));
    }
}