using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using MyWhiskyShelf.Infrastructure.Interfaces;

namespace MyWhiskyShelf.Infrastructure.Seeding;

[ExcludeFromCodeCoverage]
public class JsonFileLoader : IJsonFileLoader
{
    public async Task<List<TOut>> FetchFromJsonAsync<TOut>(string filePath, CancellationToken ct = default)
    {
        if (!File.Exists(filePath)) throw new FileNotFoundException($"'{filePath}' not found");
        if (new FileInfo(filePath).Length == 0) throw new InvalidDataException($"'{filePath}' is found, but empty");
        
        await using var fileStream = File.OpenRead(filePath);
        var result = await DeserializeFromStreamAsync<TOut>(fileStream, filePath, ct);

        return result;
    }

    private static async Task<List<TOut>> DeserializeFromStreamAsync<TOut>(
        Stream stream,
        string filePath,
        CancellationToken ct = default)
    {
        try
        {
            return await JsonSerializer.DeserializeAsync<List<TOut>>(stream, cancellationToken: ct) ?? [];
        }
        catch (JsonException ex)
        {
            throw new InvalidDataException($"'{filePath}' is found, but contains invalid data", ex);
        }
    }
}