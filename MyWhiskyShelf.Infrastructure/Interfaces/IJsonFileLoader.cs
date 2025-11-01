namespace MyWhiskyShelf.Infrastructure.Interfaces;

public interface IJsonFileLoader
{
    Task<List<TOut>> FetchFromJsonAsync<TOut>(string filePath, CancellationToken ct = default);
}