namespace MyWhiskyShelf.WebApi.Models;

public record CachedResponse(int StatusCode, string Content, string? ContentType, Dictionary<string, string?[]> Headers);