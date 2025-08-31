using System.Text.Json;
using Microsoft.Extensions.Logging;
using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.Infrastructure.Interfaces;

namespace MyWhiskyShelf.Infrastructure.Seeding;

public class JsonFileLoader(ILogger<JsonFileLoader> logger) : IJsonFileLoader
{
    private const string DistilleryPrefix = "when loading distillery data.";

    public async Task<List<Distillery>> GetDistilleriesFromJsonAsync(string filePath, CancellationToken ct = default)
    {
        if (!File.Exists(filePath)) throw new FileNotFoundException($"'{filePath}' not found {DistilleryPrefix}");

        if (new FileInfo(filePath).Length == 0)
            throw new InvalidDataException($"'{filePath}' is found, but empty, {DistilleryPrefix}");

        await using var fileStream = File.OpenRead(filePath);
        var distilleries = await LoadDistilleriesFromStreamAsync(fileStream, filePath, ct);

        logger.LogInformation("{Count} distilleries loaded", distilleries.Count);
        return distilleries;
    }

    private static async Task<List<Distillery>> LoadDistilleriesFromStreamAsync(
        Stream stream,
        string filePath,
        CancellationToken ct = default)
    {
        try
        {
            return await JsonSerializer.DeserializeAsync<List<Distillery>>(stream, cancellationToken: ct) ?? [];
        }
        catch (JsonException ex)
        {
            throw new InvalidDataException($"'{filePath}' is found, but contains invalid data, {DistilleryPrefix}", ex);
        }
    }
}