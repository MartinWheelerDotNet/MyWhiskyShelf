using System.Text.Json;
using Microsoft.Extensions.Logging;
using MyWhiskyShelf.Models;

namespace MyWhiskyShelf.Database;

public class DataLoader(ILogger<DataLoader> logger)
{
    private const string DistilleryPrefix = "when loading distillery data.";
    
    public async Task<List<Distillery>> GetDistilleriesFromJson(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"'{filePath}' not found {DistilleryPrefix}");
        }

        if (new FileInfo(filePath).Length == 0)
        {
            throw new InvalidDataException($"'{filePath}' is found, but empty, {DistilleryPrefix}");    
        }

        try
        {
            await using var fileStream = File.OpenRead(filePath);
            var distilleries = await JsonSerializer.DeserializeAsync<List<Distillery>>(fileStream) ?? [];
            
            logger.LogInformation("{Count} distilleries loaded", distilleries.Count);
            return distilleries;
        }
        catch (JsonException)
        {
            throw new InvalidDataException($"'{filePath}' is found, but contains invalid data, {DistilleryPrefix}");
        }
    } 
}


    