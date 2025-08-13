namespace MyWhiskyShelf.Database.Services;

public interface IIdempotencyService
{
    Task<Guid?> TryGetCachedResult(Guid idempotencyKey);
}