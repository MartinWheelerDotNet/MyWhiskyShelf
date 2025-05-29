using System.Text.Json;

namespace MyWhiskyShelf.Database;

public class DataLoader
{
    private const string DistilleryPrefix = "when loading distillery data.";

    public async Task<List<DistilleryData>> GetDistilleryData(string filePath)
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
            return await JsonSerializer.DeserializeAsync<List<DistilleryData>>(fileStream) ?? [];

        }
        catch (JsonException)
        {
            throw new InvalidDataException($"'{filePath}' is found, but contains invalid data, {DistilleryPrefix}");
        }
    } 
}

[Serializable]
public record DistilleryData
{
    public required string Distillery { get; init; }
    public required string Location { get; init; }
    public required string Region  { get; init; }
    public required int Founded  { get; init; }
    public required string Owner  { get; init; }
    public required string DistilleryType  { get; init; }
    public required bool Active  { get; init; }
}
    