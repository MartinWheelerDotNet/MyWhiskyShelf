using System.Text.Json;
using Microsoft.Extensions.Logging;
using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.DataLoader;

public class JsonFileLoader(ILogger<JsonFileLoader> logger) : IJsonFileLoader
{
    private const string DistilleryPrefix = "when loading distillery data.";

    public async Task<List<DistilleryRequest>> GetDistilleriesFromJsonAsync(string filePath)
    {
        if (!File.Exists(filePath)) throw new FileNotFoundException($"'{filePath}' not found {DistilleryPrefix}");

        if (new FileInfo(filePath).Length == 0)
            throw new InvalidDataException($"'{filePath}' is found, but empty, {DistilleryPrefix}");

        try
        {
            await using var fileStream = File.OpenRead(filePath);
            var distilleries = await JsonSerializer.DeserializeAsync<List<DistilleryRequest>>(fileStream) ?? [];

            logger.LogInformation("{Count} distilleries loaded", distilleries.Count);
            return distilleries;
        }
        catch (Exception)
        {
            throw new InvalidDataException($"'{filePath}' is found, but contains invalid data, {DistilleryPrefix}");
        }
    }
}