using StackExchange.Redis;

namespace MyWhiskyShelf.Database.Services;

public class RedisIdempotencyService(IConnectionMultiplexer connectionMultiplexer) : IIdempotencyService
{
    private readonly IDatabase _database = connectionMultiplexer.GetDatabase();
    public async Task<Guid?> TryGetCachedResult(Guid idempotencyKey)
    {
        var cachedResult = await _database.StringGetAsync(idempotencyKey.ToString());
        return Guid.TryParse(cachedResult, out var result) 
            ? result 
            : null;
    }
}