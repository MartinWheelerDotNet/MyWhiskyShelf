using System.Text.Json;
using MyWhiskyShelf.Models;

namespace MyWhiskyShelf.Database;

public class DataLoader
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
            return await JsonSerializer.DeserializeAsync<List<Distillery>>(fileStream) ?? [];

        }
        catch (JsonException)
        {
            throw new InvalidDataException($"'{filePath}' is found, but contains invalid data, {DistilleryPrefix}");
        }
    } 
}


    