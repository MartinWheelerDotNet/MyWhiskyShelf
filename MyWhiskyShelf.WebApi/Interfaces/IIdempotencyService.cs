using MyWhiskyShelf.WebApi.Models;

namespace MyWhiskyShelf.WebApi.Interfaces;

public interface IIdempotencyService
{
    Task<CachedResponse?> TryGetCachedResultAsync(Guid idempotencyKey);

    Task AddToCacheAsync(
        string idempotencyKey,
        int statusCode,
        string content,
        string? contentType,
        Dictionary<string, string?[]> headers);
}