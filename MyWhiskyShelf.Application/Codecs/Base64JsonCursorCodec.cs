using System.Text;
using System.Text.Json;
using MyWhiskyShelf.Application.Abstractions.Cursor;

namespace MyWhiskyShelf.Application.Codecs;

public sealed class Base64JsonCursorCodec : ICursorCodec
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public string Encode<T>(T payload)
    {
        var json = JsonSerializer.Serialize(payload, Options);
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
    }

    public bool TryDecode<T>(string? cursor, out T? payload) where T : class
    {
        payload = null;

        if (string.IsNullOrWhiteSpace(cursor))
            return true;

        try
        {
            var bytes = Convert.FromBase64String(cursor);
            var json = Encoding.UTF8.GetString(bytes);
            payload = JsonSerializer.Deserialize<T>(json, Options);
            return payload is not null;
        }
        catch
        {
            payload = null;
            return false;
        }
    }
}